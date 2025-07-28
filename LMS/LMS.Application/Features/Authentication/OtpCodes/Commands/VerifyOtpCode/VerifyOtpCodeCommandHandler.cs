using LMS.Application.Abstractions.Authentication;
using LMS.Application.Abstractions.Loggings;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.Abstractions.UnitOfWorks; // إضافة هذا الاستيراد
using LMS.Common.Enums;
using LMS.Common.Results;
using LMS.Domain.Identity.Models;
using MediatR;
using Microsoft.Extensions.Logging; // أو استخدام IAppLogger<T> إذا كان لديك تطبيق خاص به

namespace LMS.Application.Features.Authentication.OtpCodes.Commands.VerifyOtpCode
{
    public class VerifyOtpCodeCommandHandler : IRequestHandler<VerifyOtpCodeCommand, Result>
    {
        private readonly IAuthenticationHelper _authenticationHelper;
        private readonly ISoftDeletableRepository<User> _userRepo;
        private readonly IUnitOfWork _unitOfWork; // إضافة حقن IUnitOfWork
        private readonly IAppLogger<VerifyOtpCodeCommandHandler> _logger; // تغيير نوع الـ Logger ليتسق مع الكلاس

        public VerifyOtpCodeCommandHandler(
            IAuthenticationHelper authenticationHelper,
            ISoftDeletableRepository<User> userRepo,
            IUnitOfWork unitOfWork, // حقن IUnitOfWork
            IAppLogger<VerifyOtpCodeCommandHandler> logger) // حقن IAppLogger بنوع الكلاس
        {
            _authenticationHelper = authenticationHelper;
            _userRepo = userRepo;
            _unitOfWork = unitOfWork; // تهيئة
            _logger = logger; // تهيئة
        }

        public async Task<Result> Handle(VerifyOtpCodeCommand request, CancellationToken cancellationToken)
        {
            // 1. جلب المستخدم (بدون تتبع، لأننا لا نعدل كائن المستخدم نفسه هنا)
            var user = await _userRepo.GetByExpressionAsync(user => user.Email.ToLower().Trim() == request.Email.ToLower().Trim(), false);

            if (user is null)
            {
                return Result.Failure($"AUTHENTICATION.{ResponseStatus.ACCOUNT_NOT_FOUND}");
            }

            // بدء معاملة لضمان ذرية عملية التحقق وحفظ التغييرات على الـ OTP
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // 2. التحقق من الكود (دالة VerifyCodeAsync ستقوم بتعديل كائن OTP المتتبع)
                Result verifyResult = await _authenticationHelper.VerifyCodeAsync(user.UserId, request.Code, request.CodeType);

                if (verifyResult.IsFailed)
                {
                    await _unitOfWork.RollbackTransactionAsync(); // التراجع إذا فشل التحقق
                    return verifyResult;
                }

                // 3. الالتزام بجميع التغييرات المعلقة (التي تمت على كائن OTP في VerifyCodeAsync)
                await _unitOfWork.CommitTransactionAsync();

                return verifyResult; // سيعيد Result.Success من VerifyCodeAsync
            }
            catch (Exception ex)
            {
                // إذا حدث أي خطأ أثناء العملية بعد بدء المعاملة، نقوم بالتراجع
                _logger.LogError($"An unexpected error occurred during sending OTP code for user: {request.Email}", ex);
                await _unitOfWork.RollbackTransactionAsync();
                return Result.Failure($"COMMON.{ResponseStatus.UNKNOWN_ERROR}");
            }
        }
    }
}