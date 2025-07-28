using System.Text.RegularExpressions;
using AutoMapper;
using LMS.Application.Abstractions.Authentication;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.Abstractions.Services.Helpers;
using LMS.Common.Enums;
using LMS.Common.Results;
using LMS.Domain.Customers.Models;
using LMS.Domain.Customers.Models.Levels;
using LMS.Domain.Identity.Enums;
using LMS.Domain.Identity.Models;
using LMS.Infrastructure.Specifications.Authentication.Identity;

namespace LMS.Infrastructure.Services.Authentication
{
    // ملاحظة: قد تحتاج إلى تحديث أنواع ISoftDeletableRepository لتكون ISoftDeletableRepository<TEntity, Guid>
    // بناءً على التعديلات الأخيرة على واجهة المستودعات
    public class AuthenticationHelper : IAuthenticationHelper
    {
        private readonly IRandomGeneratorService _randomGenerator;
        private readonly IBaseRepository<OtpCode> _codeRepo; // تحديث النوع
        private readonly ISoftDeletableRepository<Customer> _customerRepo; // تحديث النوع
        private readonly ISoftDeletableRepository<User> _userRepo; // تحديث النوع
        private readonly ISoftDeletableRepository<LoyaltyLevel> _levelRepo; // تحديث النوع
        private readonly ISoftDeletableRepository<Role> _roleRepo; // تحديث النوع
        private readonly IMapper _mapper;
        // لا نحتاج لحقن IUnitOfWork هنا إذا كان Command Handler هو من يدير حفظ التغييرات

        public AuthenticationHelper(
            IRandomGeneratorService randomGenerator,
            IBaseRepository<OtpCode> codeRepo,
            ISoftDeletableRepository<Customer> customerRepo,
            ISoftDeletableRepository<User> userRepo, // تحديث النوع
            ISoftDeletableRepository<LoyaltyLevel> levelRepo, // تحديث النوع
            ISoftDeletableRepository<Role> roleRepo, // تحديث النوع
            IMapper mapper)
        {
            _randomGenerator = randomGenerator;
            _codeRepo = codeRepo;
            _customerRepo = customerRepo;
            _userRepo = userRepo;
            _levelRepo = levelRepo;
            _roleRepo = roleRepo;
            _mapper = mapper;
        }

        public async Task<Result<string>> GenerateAndSaveCodeAsync(Guid userId, int codeType)
        {
            // جلب الـ oldCode مع التتبع لأننا سنقوم بحذفه
            var oldCode = await _codeRepo.GetByExpressionAsync(c => c.UserId == userId, true); // <--- تفعيل التتبع

            if (oldCode is not null)
            {
                if (oldCode.CreatedAt > DateTime.UtcNow.AddMinutes(-15))
                {
                    return Result<string>.Failure($"AUTHENTICATION.OTP_CODE.{ResponseStatus.INDEFINITE_TIME_PERIOD}");
                }

                // لا حاجة لـ UpdateAsync هنا؛ إذا كان الكائن متتبعًا، EF Core سيكتشف الحذف عند SaveChangesAsync
                await _codeRepo.HardDeleteAsync(oldCode.OtpCodeId);
                // HardDeleteAsync غالبًا ما يرسل التغييرات فورًا أو يضع الكائن في حالة Deleted
                // إذا كان HardDeleteAsync يقوم بـ SaveChangesAsync() داخليًا، فهذه ملاحظة.
                // الأفضل أن تقوم HardDeleteAsync بوضع الكائن في حالة Deleted فقط.
            }

            var code = _randomGenerator.GenerateEightDigitsCode();

            var newCode = new OtpCode
            {
                HashedValue = BCrypt.Net.BCrypt.HashPassword(code),
                UserId = userId,
                CodeType = (CodeType)codeType
            };

            await _codeRepo.AddAsync(newCode);
            // لا حاجة لـ _codeRepo.UpdateAsync(newCode) هنا؛ AddAsync يضيف الكائن للتتبع.

            return Result<string>.Success(code.ToString(), $"AUTHENTICATION.OTP_CODE.{ResponseStatus.SUCCESSS_CODE_SEND}");
        }

        public async Task<Result> VerifyCodeAsync(Guid userId, string code, int codeType)
        {
            // جلب الـ OTP مع التتبع لأننا سنقوم بتعديل خصائصه
            var otp = await _codeRepo.GetByExpressionAsync(c => c.UserId == userId, true); // <--- تفعيل التتبع

            if (otp is null)
            {
                return Result.Failure($"AUTHENTICATION.OTP_CODE.{ResponseStatus.EXPIRED_CODE}");
            }

            if (otp.CodeType != (CodeType)codeType)
            {
                return Result.Failure($"AUTHENTICATION.OTP_CODE.{ResponseStatus.INVALID_CODE}");
            }

            if (otp.ExpiredAt < DateTime.UtcNow)
            {
                return Result.Failure($"AUTHENTICATION.OTP_CODE.{ResponseStatus.EXPIRED_CODE}");
            }

            if (otp.IsUsed || otp.IsVerified)
            {
                return Result.Failure($"AUTHENTICATION.OTP_CODE.{ResponseStatus.EXPIRED_CODE}");
            }

            var isMatch = BCrypt.Net.BCrypt.Verify(code, otp.HashedValue);

            if (!isMatch)
            {
                otp.FailedAttempts += 1;

                if (otp.FailedAttempts >= 3)
                {
                    otp.IsUsed = true;
                    // لا حاجة لـ _codeRepo.UpdateAsync(otp) هنا؛ التغييرات متتبعة
                    return Result.Failure($"AUTHENTICATION.OTP_CODE.{ResponseStatus.MAX_ATTEMPT}");
                }

                // لا حاجة لـ _codeRepo.UpdateAsync(otp) هنا؛ التغييرات متتبعة
                return Result.Failure($"AUTHENTICATION.{ResponseStatus.FILED_ATTEMPT}");
            }

            otp.IsVerified = true;
            // لا حاجة لـ _codeRepo.UpdateAsync(otp) هنا؛ التغييرات متتبعة

            return Result.Success("COMMON.{ResponseStatus.TASK_COMPLETED}");
        }

        public async Task<Result> CreateAndSaveAccountAsync(Customer request)
        {
            if (!IsStrongPassword(request.HashedPassword))
            {
                return Result.Failure("AUTHENTICATION.{ResponseStatus.WEAK_PASSWORD}");
            }

            // لا حاجة للتتبع هنا لأننا فقط نتحقق من الوجود
            var oldUser = await _userRepo.GetByExpressionAsync(user =>
                user.Email.ToLower().Trim() == request.Email.ToLower().Trim() ||
                user.PhoneNumber.Trim() == request.PhoneNumber.Trim() ||
                user.UserName.Trim() == request.UserName.Trim(), false); // <--- تعطيل التتبع

            if (oldUser is not null)
            {
                return Result.Failure($"AUTHENTICATION.{ResponseStatus.ALREADY_EXISTING_ACCOUNT}");
            }
                
            var newCustomer = _mapper.Map<Customer>(request);
            newCustomer.HashedPassword = BCrypt.Net.BCrypt.HashPassword(request.HashedPassword);

            // جلب الأدوار والمستويات بدون تتبع، ثم تعيين المفاتيح الأجنبية فقط
            var customersRole = await _roleRepo.GetByExpressionAsync(
                role => role.RoleType.ToLower() == "customer", false); // <--- تعطيل التتبع

            if (customersRole is null)
            {
                return Result.Failure($"COMMON.{ResponseStatus.UNKNOWN_ERROR}");
            }

            var firstLevel = await _levelRepo.GetByExpressionAsync(
                level => level.RequiredPoints == 0, false); // <--- تعطيل التتبع

            if (firstLevel is null)
            {
                return Result.Failure($"COMMON.{ResponseStatus.UNKNOWN_ERROR}");
            }

            // تعيين المفاتيح الأجنبية فقط. EF Core سيتعامل مع العلاقات.
            newCustomer.LevelId = firstLevel.LevelId;
            newCustomer.RoleId = customersRole.RoleId;

            await _customerRepo.AddAsync(newCustomer);
            // لا حاجة لـ _customerRepo.UpdateAsync(newCustomer) هنا؛ AddAsync يضيف للتتبع.

            return Result.Success($"COMMON.{ResponseStatus.TASK_COMPLETED}");
        }

        public async Task<Result<Guid>> CanActivateAuth(string email, int activationType)
        {
            // جلب المستخدم مع التتبع لأنه سيتم تعديل خصائص user و otp
            var user = await _userRepo.GetBySpecificationAsync(new UserWithOtpCodeSpecification(email));
            // ملاحظة: يجب التأكد أن UserWithOtpCodeSpecification يدعم isTrackingEnabled

            if (user is null)
            {
                return Result<Guid>.Failure($"AUTHENTICATION.{ResponseStatus.ACCOUNT_NOT_FOUND}");
            }

            var otp = user.OtpCode;

            if (otp is null)
            {
                return Result<Guid>.Failure($"AUTHENTICATION.{ResponseStatus.ACTIVATION_FAILED}");
            }

            if (otp.CodeType != (CodeType)activationType || otp.IsUsed || !otp.IsVerified)
            {
                return Result<Guid>.Failure($"AUTHENTICATION.OTP_CODE.{ResponseStatus.EXPIRED_CODE}");
            }

            if (otp.CodeType == CodeType.AccountActivation)
            {
                user.IsEmailConfirmed = true;
            }

            otp.IsUsed = true;
            user.LastLogIn = DateTime.UtcNow;

            // لا حاجة لـ _userRepo.UpdateAsync(user) أو _codeRepo.UpdateAsync(otp)
            // التغييرات على user و otp متتبعة بالفعل بواسطة EF Core.

            return Result<Guid>.Success(user.UserId, $"AUTHENTICATION.{ResponseStatus.ACTIVATION_SUCCESS}");
        }

        public Result LogIn(User user, string password)
        {
            if (user.IsDeleted)
            {
                return Result.Failure($"AUTHENTICATION.{ResponseStatus.ACCOUNT_NOT_FOUND}");
            }

            if (user.IsLocked && user.LockedUntil > DateTime.UtcNow) // <--- تصحيح الشرط
            {
                return Result.Failure($"AUTHENTICATION.{ResponseStatus.LOCKED_ACCOUNT}");
            }

            if (!user.IsEmailConfirmed)
            {
                return Result.Failure($"AUTHENTICATION.{ResponseStatus.UNVERIFIED_ACCOUNT}");
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.HashedPassword))
            {
                user.FailedLoginAttempts++;

                if (user.FailedLoginAttempts >= 3)
                {
                    user.IsLocked = true;
                    user.LockedUntil = DateTime.UtcNow.AddMinutes(15);
                    // لا حاجة لـ _userRepo.UpdateAsync(user) هنا؛ التغييرات متتبعة
                    return Result.Failure($"AUTHENTICATION.{ResponseStatus.MAX_LOGIN_ATTEMPTS}");
                }
                // لا حاجة لـ _userRepo.UpdateAsync(user) هنا؛ التغييرات متتبعة
                return Result.Failure($"AUTHENTICATION.{ResponseStatus.FILED_ATTEMPT}");
            }

            user.FailedLoginAttempts = 0;
            user.LastLogIn = DateTime.UtcNow;
            user.IsLocked = false;
            user.LockedUntil = null;

            // لا حاجة لـ _userRepo.UpdateAsync(user) هنا؛ التغييرات متتبعة

            // ملاحظة: هذا الشرط تم التعامل معه بشكل أفضل في LogInCommandHandler
            // إذا كان المستخدم نشطًا بـ 2FA، لا يجب أن يفشل تسجيل الدخول هنا،
            // بل يجب أن يستمر تدفق 2FA.
            // ولكن بناءً على الكود الأصلي، نترك هذا كما هو في AuthenticationHelper.
            if (user.IsTwoFactorEnabled)
            {
                return Result.Failure($"AUTHENTICATION.{ResponseStatus.AUTHENTICATION_FAILED}");
            }

            return Result.Success($"AUTHENTICATION.{ResponseStatus.AUTHENTICATION_SUCCESS}");
        }

        // هذا الكود لم يتغير
        private bool IsStrongPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            if (password.Length < 8)
                return false;

            if (!Regex.IsMatch(password, "[A-Z]"))
                return false;

            if (!Regex.IsMatch(password, "[a-z]"))
                return false;

            if (!Regex.IsMatch(password, "[0-9]"))
                return false;

            return true;
        }
    }
}