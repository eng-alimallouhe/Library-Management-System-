using LMS.Application.Abstractions.Authentication;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.Abstractions.UnitOfWorks; // إضافة هذا الاستيراد
using LMS.Application.DTOs.Authentication;
using LMS.Common.Enums;
using LMS.Common.Results;
using LMS.Domain.Identity.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Authentication.Accounts.Commands.LogIn
{
    public class LogInCommandHandler : IRequestHandler<LogInCommand, Result<AuthorizationDto>>
    {
        private readonly IAuthenticationHelper _authenticationHelper;
        private readonly ISoftDeletableRepository<User> _userRepo; // تحديث النوع ليتناسب مع IBaseRepository<TEntity, TKey>
        private readonly ITokenGeneratorService _tokenGeneratorService;
        private readonly ILogger<LogInCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork; // إضافة حقن IUnitOfWork

        public LogInCommandHandler(
            IAuthenticationHelper authenticationHelper,
            ISoftDeletableRepository<User> userRepo, // تحديث النوع
            ITokenGeneratorService tokenGeneratorService,
            ILogger<LogInCommandHandler> logger,
            IUnitOfWork unitOfWork) // إضافة حقن
        {
            _authenticationHelper = authenticationHelper;
            _userRepo = userRepo;
            _tokenGeneratorService = tokenGeneratorService;
            _logger = logger;
            _unitOfWork = unitOfWork; // تهيئة
        }

        public async Task<Result<AuthorizationDto>> Handle(LogInCommand request, CancellationToken cancellationToken)
        {
            // 1. جلب المستخدم مع تفعيل التتبع لأننا سنقوم بتعديل بياناته لاحقًا
            var user = await _userRepo.GetByExpressionAsync(u => u.Email.ToLower().Trim() == request.Email.ToLower().Trim(), true);

            if (user is null)
            {
                // لا حاجة لـ Rollback هنا لأن لم يتم بدء أي معاملة أو تغيير أي شيء بعد
                return Result<AuthorizationDto>.Failure($"AUTHENTICATION.{ResponseStatus.ACCOUNT_NOT_FOUND}");
            }

            // بدء معاملة لضمان ذرية جميع التغييرات اللاحقة (تحديثات المستخدم، إنشاء التوكنات)
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                Result loginResult =  _authenticationHelper.LogIn(user, request.Password);

                if (loginResult.IsFailed)
                {
                    await _unitOfWork.RollbackTransactionAsync(); // التراجع في حالة فشل تسجيل الدخول
                    return Result<AuthorizationDto>.Failure(loginResult.StatusKey);
                }

                // **تعديل منطق 2FA:** إذا كان التوثيق الثنائي مفعلاً، فإن المصادقة لم تكتمل بعد.
                // يجب أن يشير هذا إلى أن خطوة إضافية مطلوبة بدلاً من الفشل.
                // هنا، سنبسط الأمر ونرجعه كفشل يشير إلى 2FA مطلوب.
                // في تطبيق حقيقي، قد يكون لديك نوع Result خاص أو AuthorizationDto يحتوي على flags.
                if (user.IsTwoFactorEnabled)
                {
                    // قد تحتاج هنا لإرسال رمز 2FA إلى المستخدم
                    // هذا المثال لا يقوم بذلك، بل يفشل تسجيل الدخول
                    await _unitOfWork.RollbackTransactionAsync();
                    return Result<AuthorizationDto>.Failure($"AUTHENTICATION.{ResponseStatus.TWO_FACTOR_REQUIRED}");
                }

                var refreshResult = await _tokenGeneratorService.GenerateRefreshTokenAsync(user.UserId);
                if (refreshResult.IsFailed || refreshResult.Value is null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return Result<AuthorizationDto>.Failure(refreshResult.StatusKey);
                }

                var accessResult = await _tokenGeneratorService.GenerateAccessTokenAsync(user.UserId);
                if (accessResult.IsFailed || accessResult.Value is null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return Result<AuthorizationDto>.Failure(refreshResult.StatusKey);
                }

                // بما أن المستخدم تم جلبه بـ Tracking = true، فإن أي تغييرات على الكائن 'user'
                // قد تم تتبعها بواسطة EF Core.
                // _authenticationHelper.LogIn() قام بتعديل خصائص المستخدم (مثل FailedLoginAttempts, LastLogIn).
                // الآن نقوم بالالتزام بجميع هذه التغييرات والمعاملة.
                await _unitOfWork.CommitTransactionAsync(); // هذا سيتضمن SaveChangesAsync()

                return Result<AuthorizationDto>.Success(new AuthorizationDto
                {
                    AccessToken = accessResult.Value,
                    RefreshToken = refreshResult.Value
                }, $"AUTHENTICATION.{ResponseStatus.AUTHENTICATION_SUCCESS}");
            }
            catch (Exception ex)
            {
                // إذا حدث أي خطأ أثناء العملية بعد بدء المعاملة، نقوم بالتراجع
                _logger.LogError(ex, "An error occurred during login process for user: {Email}", request.Email);
                await _unitOfWork.RollbackTransactionAsync();
                return Result<AuthorizationDto>.Failure($"COMMON.{ResponseStatus.UNKNOWN_ERROR}");
            }
        }
    }
}