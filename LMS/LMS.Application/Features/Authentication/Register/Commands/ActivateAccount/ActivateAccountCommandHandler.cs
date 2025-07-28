using LMS.Application.Abstractions.Authentication;
using LMS.Application.Abstractions.Repositories; // هذا قد يظل مطلوباً إذا كان هناك استخدامات أخرى
using LMS.Application.Abstractions.UnitOfWorks; // إضافة هذا الاستيراد
using LMS.Application.DTOs.Authentication;
using LMS.Common.Enums;
using LMS.Common.Results;
using LMS.Domain.Identity.Enums;
using MediatR;
using Microsoft.Extensions.Logging; // أو IAppLogger<T> إذا كان لديك تطبيق خاص به

namespace LMS.Application.Features.Authentication.Register.Commands.ActivateAccount
{
    public class ActivateAccountCommandHandler : IRequestHandler<ActivateAccountCommand, Result<AuthorizationDto>>
    {
        private readonly IAuthenticationHelper _authenticationHelper;
        private readonly ITokenGeneratorService _tokenGeneratorService;
        private readonly IUnitOfWork _unitOfWork; // إضافة حقن IUnitOfWork
        private readonly ILogger<ActivateAccountCommandHandler> _logger; // إضافة حقن ILogger

        public ActivateAccountCommandHandler(
            IAuthenticationHelper authenticationHelper,
            // ISoftDeletableRepository<User> userRepo, // <--- إزالة هذا الحقن إذا لم يكن مستخدماً مباشرةً
            // IBaseRepository<OtpCode> codeRepo,     // <--- إزالة هذا الحقن إذا لم يكن مستخدماً مباشرةً
            ILogger<ActivateAccountCommandHandler> logger, // الحفاظ عليه واستخدامه
            IUnitOfWork unitOfWork, // حقن IUnitOfWork
            ITokenGeneratorService tokenGeneratorService)
        {
            _authenticationHelper = authenticationHelper;
            _tokenGeneratorService = tokenGeneratorService;
            _unitOfWork = unitOfWork; // تهيئة
            _logger = logger; // تهيئة
        }

        public async Task<Result<AuthorizationDto>> Handle(ActivateAccountCommand request, CancellationToken cancellationToken)
        {
            // 1. تفعيل الحساب / التحقق من OTP
            // ملاحظة: دالة CanActivateAuth في AuthenticationHelper تقوم بتعديل حالة User (IsEmailConfirmed, LastLogIn) والـ OTP
            Result<Guid> authResult = await _authenticationHelper.CanActivateAuth(request.Email, (int)CodeType.AccountActivation);

            if (authResult.IsFailed)
            {
                // لا حاجة لـ Rollback هنا لأنه لم يتم بدء أي معاملة بعد أو لم يتم تعديل أي شيء
                return Result<AuthorizationDto>.Failure(authResult.StatusKey);
            }

            var userId = authResult.Value;

            // بدء معاملة لضمان ذرية جميع التغييرات اللاحقة (تحديثات المستخدم/OTP وإنشاء التوكنات)
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // **ملاحظة:** دالة `CanActivateAuth` قامت بتعديل `user.IsEmailConfirmed` و `otp.IsUsed` و `user.LastLogIn`
                // هذه التغييرات تم تتبعها بواسطة EF Core في الـ `DbContext` الخاص بـ `IUnitOfWork`.

                // 2. إنشاء Refresh Token
                var refreshResult = await _tokenGeneratorService.GenerateRefreshTokenAsync(userId);
                if (refreshResult.IsFailed || refreshResult.Value is null)
                {
                    await _unitOfWork.RollbackTransactionAsync(); // التراجع في حالة فشل إنشاء التوكن
                    return Result<AuthorizationDto>.Failure(refreshResult.StatusKey);
                }

                // 3. إنشاء Access Token
                var accessResult = await _tokenGeneratorService.GenerateAccessTokenAsync(userId);
                if (accessResult.IsFailed || accessResult.Value is null)
                {
                    await _unitOfWork.RollbackTransactionAsync(); // التراجع في حالة فشل إنشاء التوكن
                    return Result<AuthorizationDto>.Failure(accessResult.StatusKey);
                }

                // الالتزام بجميع التغييرات المعلقة (التي تمت في CanActivateAuth) وإنشاء التوكنات
                await _unitOfWork.CommitTransactionAsync(); // هذا سيستدعي SaveChangesAsync() داخليًا

                return Result<AuthorizationDto>.Success(new AuthorizationDto
                {
                    RefreshToken = refreshResult.Value,
                    AccessToken = accessResult.Value,
                }, $"AUTHENTICATION.{ResponseStatus.ACTIVATION_SUCCESS}");
            }
            catch (Exception ex)
            {
                // إذا حدث أي خطأ أثناء العملية بعد بدء المعاملة، نقوم بالتراجع
                _logger.LogError(ex, "An error occurred during account activation process for user ID: {UserId}", userId);
                await _unitOfWork.RollbackTransactionAsync();
                return Result<AuthorizationDto>.Failure($"COMMON.{ResponseStatus.UNKNOWN_ERROR}");
            }
        }
    }
}