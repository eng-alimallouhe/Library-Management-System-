using AutoMapper;
using LMS.Application.Abstractions.Repositories;
using LMS.Common.Enums;
using LMS.Common.Results;
using LMS.Domain.HR.Models;
using LMS.Domain.Identity.Models;
using MediatR;
using LMS.Application.Abstractions.UnitOfWorks;
using LMS.Application.Abstractions.Loggings; 
using LMS.Application.Abstractions.HR;
using LMS.Application.Abstractions.Authentication;
using LMS.Application.Abstractions.Common;

namespace LMS.Application.Features.HR.Employees.Command.UpdateEmployee
{
    public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, Result>
    {
        private readonly ISoftDeletableRepository<Employee> _employeeRepo;
        private readonly ISoftDeletableRepository<User> _userRepo;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork; // حقن Unit of Work
        private readonly IAppLogger<UpdateEmployeeCommandHandler> _logger; // حقن IAppLogger
        private readonly IFaceRecognitionHelper _faceRecognitionHelper; // حقن Face Recognition Helper
        private readonly IConverterHelper _converterHelper; // حقن Converter Helper

        public UpdateEmployeeCommandHandler(
            ISoftDeletableRepository<Employee> employeeRepo,
            ISoftDeletableRepository<User> userRepo,
            IMapper mapper,
            IUnitOfWork unitOfWork, // إضافة IUnitOfWork للـ constructor
            IAppLogger<UpdateEmployeeCommandHandler> logger, // إضافة IAppLogger للـ constructor
            IFaceRecognitionHelper faceRecognitionHelper, // إضافة IFaceRecognitionHelper
            IConverterHelper converterHelper) // إضافة IConverterHelper
        {
            _userRepo = userRepo;
            _employeeRepo = employeeRepo;
            _mapper = mapper;
            _unitOfWork = unitOfWork; // تهيئة Unit of Work
            _logger = logger; // تهيئة Logger
            _faceRecognitionHelper = faceRecognitionHelper;
            _converterHelper = converterHelper;
        }

        public async Task<Result> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            // بدء المعاملة لضمان ذرية عملية التحديث
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // 1. جلب بيانات الموظف (مع التتبع لتحديثها)
                var employeeToUpdate = await _employeeRepo.GetByIdAsync(request.EmployeeId); // <--- جلب مع التتبع

                if (employeeToUpdate is null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    _logger.LogWarning($"Employee with ID {request.EmployeeId} not found for update."); // لا يوجد استثناء هنا
                    return Result.Failure($"HR.EMPLOYEES.{ResponseStatus.EMPLOYEE_NOT_FOUNDED}");
                }

                // 2. التحقق من تكرار البريد الإلكتروني
                // إذا تم تغيير البريد الإلكتروني، نتحقق من عدم وجود مستخدم آخر بنفس البريد
                if (employeeToUpdate.Email.ToLower().Trim() != request.Email.ToLower().Trim())
                {
                    var existingUserWithNewEmail = await _userRepo.GetByExpressionAsync(
                        user => user.Email.ToLower().Trim() == request.Email.ToLower().Trim(), false); // <--- بدون تتبع

                    if (existingUserWithNewEmail is not null && existingUserWithNewEmail.UserId != employeeToUpdate.UserId)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        _logger.LogWarning($"Another user with email {request.Email} already exists. Cannot update employee {request.EmployeeId}.");
                        return Result.Failure($"AUTHENTICATION.{ResponseStatus.EXISTING_ACCOUNT}");
                    }
                }

                // 3. تحديث بيانات الموظف باستخدام AutoMapper
                _mapper.Map(request, employeeToUpdate);

                // 4. تحديث الصورة الشخصية (إن وجدت)
                if (request.FaceImage is not null && request.FaceImage.Length > 0)
                {
                    var faceVector = _faceRecognitionHelper.ExtractFaceEncoding(request.FaceImage);

                    if (faceVector is null)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        _logger.LogError($"Failed to extract face features from provided image for employee {request.EmployeeId}.", null); // لا يوجد استثناء مباشر من الـ helper
                        return Result.Failure($"COMMON.{ResponseStatus.FACE_EXTRACTION_FAILED}"); // رسالة خطأ أكثر تحديدًا
                    }

                    employeeToUpdate.FaceFeatureVector = _converterHelper.ConvertDoubleArrayToBytes(faceVector);
                }
                // إذا لم يتم تمرير صورة، لن نغير FaceFeatureVector، وسيبقى القديم كما هو.

                employeeToUpdate.UpdatedAt = DateTime.UtcNow; // تحديث تاريخ آخر تعديل

                // لا حاجة لاستدعاء _employeeRepo.UpdateAsync(employeeToUpdate) بشكل صريح هنا
                // لأن الكيان employeeToUpdate متتبع بواسطة EF Core، وسيتم اكتشاف التغييرات عند Commit.

                // 5. الالتزام بجميع التغييرات المعلقة
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
                _logger.LogInformation($"Employee {request.EmployeeId} updated successfully.");
                return Result.Success($"COMMON.{ResponseStatus.TASK_COMPLETED}");
            }
            catch (Exception ex)
            {
                // إذا حدث أي خطأ غير متوقع، نقوم بالتراجع وتسجيل الخطأ
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError($"An unexpected error occurred during employee update for EmployeeId: {request.EmployeeId}", ex);
                return Result.Failure($"COMMON.{ResponseStatus.UNKNOWN_ERROR}");
            }
        }
    }
}