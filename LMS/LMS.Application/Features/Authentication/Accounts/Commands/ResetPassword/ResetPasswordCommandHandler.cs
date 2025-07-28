using LMS.Application.Abstractions.Authentication;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.Abstractions.UnitOfWorks;
using LMS.Application.DTOs.Authentication;
using LMS.Common.Enums;
using LMS.Common.Results;
using LMS.Domain.Identity.Enums;
using LMS.Domain.Identity.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Authentication.Accounts.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result<AuthorizationDto>>
    {
        private readonly IAuthenticationHelper _authenticationHelper;
        private readonly ITokenGeneratorService _tokenGeneratorService;
        private readonly ISoftDeletableRepository<User> _userRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ResetPasswordCommandHandler> _logger;

        public ResetPasswordCommandHandler(
            IAuthenticationHelper authenticationHelper,
            ITokenGeneratorService tokenGeneratorService,
            ISoftDeletableRepository<User> userRepo,
            IUnitOfWork unitOfWork,
            ILogger<ResetPasswordCommandHandler> logger)
        {
            _authenticationHelper = authenticationHelper;
            _tokenGeneratorService = tokenGeneratorService;
            _userRepo = userRepo;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<AuthorizationDto>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            // 1. التحقق من صحة رمز إعادة تعيين كلمة المرور
            // ملاحظة: دالة CanActivateAuth في AuthenticationHelper تقوم بتعديل حالة OTP و User (بما في ذلك LastLogIn)
            var authResult = await _authenticationHelper.CanActivateAuth(request.Email, (int)CodeType.ResetPassword);

            if (authResult.IsFailed)
            {
                return Result<AuthorizationDto>.Failure(authResult.StatusKey);
            }

            var userId = authResult.Value;

            // 2. جلب المستخدم لتحديث كلمة المرور
            // يجب جلب المستخدم مع التتبع لأننا سنقوم بتعديل خصائصه
            // ملاحظة: المستخدم الذي تم جلبه في CanActivateAuth هو نفس الكائن المتتبع
            // لذا يمكننا الاستمرار في تعديله.
            var user = await _userRepo.GetByIdAsync(userId);

            if (user is null)
            {
                // هذا الشرط من الناحية النظرية لا ينبغي أن يحدث
                // لأن CanActivateAuth قد وجدت المستخدم بالفعل.
                return Result<AuthorizationDto>.Failure($"AUTHENTICATION.{ResponseStatus.ACCOUNT_NOT_FOUND}");
            }

            // بدء معاملة لضمان ذرية جميع التغييرات اللاحقة
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // 3. تحديث كلمة المرور الجديدة
                user.HashedPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

                // **تمت إزالة هذا السطر:** user.LastLogIn = DateTime.UtcNow;
                // لأنه تم تحديثه بالفعل في AuthenticationHelper.CanActivateAuth()

                // 4. توليد توكنات الوصول والتحديث
                var refreshResult = await _tokenGeneratorService.GenerateRefreshTokenAsync(userId);
                if (refreshResult.IsFailed || refreshResult.Value is null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return Result<AuthorizationDto>.Failure(refreshResult.StatusKey);
                }

                var accessResult = await _tokenGeneratorService.GenerateAccessTokenAsync(userId);
                if (accessResult.IsFailed || accessResult.Value is null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return Result<AuthorizationDto>.Failure(refreshResult.StatusKey);
                }

                // الالتزام بجميع التغييرات المعلقة والمعاملة
                await _unitOfWork.CommitTransactionAsync();

                return Result<AuthorizationDto>.Success(new AuthorizationDto
                {
                    RefreshToken = refreshResult.Value,
                    AccessToken = accessResult.Value
                }, $"AUTHENTICATION.{ResponseStatus.AUTHENTICATION_SUCCESS}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during password reset process for user ID: {UserId}", userId);
                await _unitOfWork.RollbackTransactionAsync();
                return Result<AuthorizationDto>.Failure($"COMMON.{ResponseStatus.UNKNOWN_ERROR}");
            }
        }
    }
}