using AutoMapper;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.Abstractions.UnitOfWorks; // إضافة هذا الاستيراد
using LMS.Application.Abstractions.Loggings; // إضافة هذا الاستيراد لواجهة IAppLogger
using LMS.Common.Enums;
using LMS.Common.Results;
using LMS.Domain.HR.Models;
using MediatR;

namespace LMS.Application.Features.HR.Departments.Command.UpdateDepartment
{
    public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, Result>
    {
        private readonly ISoftDeletableRepository<Department> _departmentRepo;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork; // حقن IUnitOfWork
        private readonly IAppLogger<UpdateDepartmentCommandHandler> _logger; // حقن IAppLogger

        public UpdateDepartmentCommandHandler(
            ISoftDeletableRepository<Department> departmentRepo,
            IMapper mapper,
            IUnitOfWork unitOfWork, // إضافة IUnitOfWork إلى constructor
            IAppLogger<UpdateDepartmentCommandHandler> logger) // إضافة IAppLogger إلى constructor
        {
            _departmentRepo = departmentRepo;
            _mapper = mapper;
            _unitOfWork = unitOfWork; // تهيئة IUnitOfWork
            _logger = logger; // تهيئة IAppLogger
        }

        public async Task<Result> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
        {
            // بدء معاملة لضمان ذرية عملية تحديث القسم
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // 1. جلب القسم الموجود مع تفعيل التتبع (`true`)
                // استخدام GetByIdAsync أفضل عندما يكون لديك ID
                var departmentToUpdate = await _departmentRepo.GetByIdAsync(request.DepartmentId);

                if (departmentToUpdate is null)
                {
                    await _unitOfWork.RollbackTransactionAsync(); // التراجع إذا لم يتم العثور على القسم
                    return Result.Failure($"DEPARTMENTS.{ResponseStatus.DEPARTMENT_NOT_FOUNDED}");
                }

                // 2. التحقق من عدم وجود قسم آخر بنفس الاسم الجديد، إلا إذا كان هو نفس القسم الذي نعدله
                if (departmentToUpdate.DepartmentName.ToLower().Trim() != request.DepartmentName.ToLower().Trim())
                {
                    var existingDepartmentWithSameName = await _departmentRepo.GetByExpressionAsync(
                        d => d.DepartmentName.ToLower().Trim() == request.DepartmentName.ToLower().Trim(), false); // بدون تتبع

                    if (existingDepartmentWithSameName is not null)
                    {
                        await _unitOfWork.RollbackTransactionAsync(); // التراجع في حالة وجود اسم مكرر
                        return Result.Failure($"HR.DEPARTMENT.{ResponseStatus.ALREADE_EXIST_RECORD}");
                    }
                }

                // 3. تعيين القيم الجديدة للكيان المتتبع (AutoMapper سيحدث الخصائص)
                _mapper.Map(request, departmentToUpdate);

                // 4. لا حاجة لاستدعاء _departmentRepo.UpdateAsync(departmentToUpdate) بشكل صريح هنا
                // لأن الكيان departmentToUpdate متتبع بالفعل بواسطة EF Core، وسيتم اكتشاف التغييرات عند Commit.
                // إذا كانت دالة UpdateAsync في المستودع تقوم بعمليات إضافية (مثل تسجيل التغييرات)، فيمكن استدعاؤها.
                // ولكن لتحديث بسيط لكيان متتبع، لا حاجة لاستدعاء صريح.

                // 5. الالتزام بجميع التغييرات المعلقة
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return Result.Success($"COMMON.{ResponseStatus.UPDATE_COMPLETED}");
            }
            catch (Exception ex)
            {
                // إذا حدث أي خطأ أثناء العملية بعد بدء المعاملة، نقوم بالتراجع وتسجيل الخطأ
                _logger.LogError("An error occurred during department update for ID: {DepartmentId}", ex);
                await _unitOfWork.RollbackTransactionAsync();
                return Result.Failure($"COMMON.{ResponseStatus.UNKNOWN_ERROR}");
            }
        }
    }
}