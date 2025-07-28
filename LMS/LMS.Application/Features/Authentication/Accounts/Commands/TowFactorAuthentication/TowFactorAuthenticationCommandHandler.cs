using LMS.Application.Abstractions.Authentication;
using LMS.Application.Abstractions.UnitOfWorks; // إضافة هذا الاستيراد
using LMS.Application.DTOs.Authentication;
using LMS.Common.Enums;
using LMS.Common.Results;
using LMS.Domain.Identity.Enums;
using MediatR;
using Microsoft.Extensions.Logging; // إضافة ILogger للحصول على رؤى أفضل للأخطاء

namespace LMS.Application.Features.Authentication.Accounts.Commands.TowFactorAuthentication
{
    public class TowFactorAuthenticationCommandHandler : IRequestHandler<TowFactorAuthenticationCommand, Result<AuthorizationDto>>
    {
        private readonly IAuthenticationHelper _authenticationHelper;
        private readonly ITokenGeneratorService _tokenGeneratorService;
        private readonly IUnitOfWork _unitOfWork; // إضافة حقن IUnitOfWork
        private readonly ILogger<TowFactorAuthenticationCommandHandler> _logger; // إضافة ILogger

        public TowFactorAuthenticationCommandHandler(
            IAuthenticationHelper authenticationHelper,
            ITokenGeneratorService tokenGeneratorService,
            IUnitOfWork unitOfWork, // حقن IUnitOfWork
            ILogger<TowFactorAuthenticationCommandHandler> logger) // حقن ILogger
        {
            _authenticationHelper = authenticationHelper;
            _tokenGeneratorService = tokenGeneratorService;
            _unitOfWork = unitOfWork; // تهيئة
            _logger = logger; // تهيئة
        }

        public async Task<Result<AuthorizationDto>> Handle(TowFactorAuthenticationCommand request, CancellationToken cancellationToken)
        {
            // لا حاجة لبدء معاملة قبل استدعاء CanActivateAuth
            // لأن CanActivateAuth هي مجرد دالة مساعدة تقوم بتعديل كيانات متتبعة.
            // الـ Commit/Rollback سيتم في CommandHandler.

            // 1. تفعيل الحساب / التحقق من OTP
            var authResult = await _authenticationHelper.CanActivateAuth(request.Email, (int)CodeType.TowFA);

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
                    AccessToken = accessResult.Value
                }, $"AUTHENTICATION.{ResponseStatus.AUTHENTICATION_SUCCESS}");
            }
            catch (Exception ex)
            {
                // إذا حدث أي خطأ أثناء العملية بعد بدء المعاملة، نقوم بالتراجع
                _logger.LogError(ex, "An error occurred during two-factor authentication process for user ID: {UserId}", userId);
                await _unitOfWork.RollbackTransactionAsync();
                return Result<AuthorizationDto>.Failure($"COMMON.{ResponseStatus.UNKNOWN_ERROR}");
            }
        }
    }
}