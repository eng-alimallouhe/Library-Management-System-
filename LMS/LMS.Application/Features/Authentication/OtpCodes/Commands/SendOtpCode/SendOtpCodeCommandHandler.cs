using LMS.Application.Abstractions.Authentication;
using LMS.Application.Abstractions.Communications;
using LMS.Application.Abstractions.Loggings;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.Abstractions.UnitOfWorks;
using LMS.Common.Enums;
using LMS.Common.Exceptions; // تأكد من أنك تستخدم هذا النوع للاستثناءات
using LMS.Common.Results;
using LMS.Domain.Identity.Models;
using MediatR;
using Microsoft.Extensions.Logging; // لاحظ أنك تستخدم IAppLogger الخاص بك، لكن ILogger جيد أيضاً.

namespace LMS.Application.Features.Authentication.OtpCodes.Commands.SendOtpCode
{
    public class SendOtpCodeCommandHandler : IRequestHandler<SendOtpCodeCommand, Result>
    {
        private readonly IAuthenticationHelper _authenticationHelper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSenderService _emailSenderService;
        private readonly ISoftDeletableRepository<User> _userRepo; // نوع المستودع
        private readonly IAppLogger<SendOtpCodeCommandHandler> _logger;
        private readonly IEmailTemplateReaderService _emailTemplateReaderService;


        public SendOtpCodeCommandHandler(
            IAuthenticationHelper authenticationHelper,
            IUnitOfWork unitOfWork,
            IEmailSenderService emailSenderService,
            ISoftDeletableRepository<User> userRepo, // نوع المستودع
                                                     // IBaseRepository<OtpCode> codeRepo, // <--- إزالة هذا الحقن، غير مستخدم هنا
            IEmailTemplateReaderService emailTemplateReaderService,
            IAppLogger<SendOtpCodeCommandHandler> logger)
        {
            _authenticationHelper = authenticationHelper;
            _unitOfWork = unitOfWork;
            _emailSenderService = emailSenderService;
            _userRepo = userRepo;
            _logger = logger;
            _emailTemplateReaderService = emailTemplateReaderService;
        }

        public async Task<Result> Handle(SendOtpCodeCommand request, CancellationToken cancellationToken)
        {
            var purpose = (EmailPurpose)(int)request.CodeType;

            // جلب المستخدم بدون تتبع لأننا لا نعدل كائن المستخدم مباشرة هنا
            var user = await _userRepo.GetByExpressionAsync(user => user.Email.ToLower().Trim() == request.Email.ToLower().Trim(), false); // <--- تعطيل التتبع

            if (user is null)
            {
                return Result.Failure($"AUTHENTICATION.{ResponseStatus.ACCOUNT_NOT_FOUND}");
            }

            var template = _emailTemplateReaderService.ReadTemplate(user.Language, purpose);

            if (template is null)
            {
                return Result.Failure($"COMMON.{ResponseStatus.UNKNOWN_ERROR}");
            }

            // بدء المعاملة قبل أي عمليات تغيير قاعدة بيانات
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // توليد وحفظ الكود (سيتم تتبع التغييرات بواسطة AuthenticationHelper)
                var codeResult = await _authenticationHelper.GenerateAndSaveCodeAsync(user.UserId, request.CodeType);

                if (codeResult.IsFailed || codeResult.Value is null)
                {
                    await _unitOfWork.RollbackTransactionAsync(); // التراجع في حالة فشل توليد/حفظ الكود
                    return Result.Failure(codeResult.StatusKey);
                }

                // تجهيز قالب البريد الإلكتروني
                string body = template.Replace("{{name}}", user.UserName)
                                        .Replace("{{code}}", codeResult.Value);

                int currentAttempt = 0;
                bool isSended = false;

                // حلقة لإعادة محاولة إرسال البريد الإلكتروني
                while (currentAttempt < 3 && !isSended) // <--- الشرط هنا يجب أن يكون < 3 للمحاولة 3 مرات
                {
                    try
                    {
                        await _emailSenderService.SendEmailAsync(request.Email, $"Verify {request.CodeType}", body);
                        isSended = true;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error sending OTP code for user: {user.UserName}, attempt {currentAttempt + 1}", ex);
                        currentAttempt++;
                        if (currentAttempt < 3) // فقط إذا لم تكن المحاولة الأخيرة
                        {
                            await Task.Delay(1000).ConfigureAwait(false); // انتظر قليلاً قبل المحاولة التالية
                        }
                    }
                }

                if (!isSended)
                {
                    await _unitOfWork.RollbackTransactionAsync(); // التراجع إذا فشل إرسال البريد بعد كل المحاولات
                    _logger.LogError($"Failed to send OTP code after 3 attempts for user: {user.UserName}", new Exception());
                    return Result.Failure($"COMMUNICATION.{ResponseStatus.EMAIL_SEND_FAILED}"); // رسالة خطأ أكثر تحديداً
                }

                // الالتزام بجميع التغييرات المعلقة والمعاملة
                await _unitOfWork.CommitTransactionAsync();

                return Result.Success($"AUTHENTICATION.OTP_CODE.{ResponseStatus.SUCCESSS_CODE_SEND}");
            }
            catch (Exception ex)
            {
                // التقاط أي استثناءات أخرى غير متوقعة
                _logger.LogError($"An unexpected error occurred during sending OTP code for user: {user.UserName}", ex);
                await _unitOfWork.RollbackTransactionAsync();
                return Result.Failure($"COMMON.{ResponseStatus.UNKNOWN_ERROR}");
            }
        }
    }
}