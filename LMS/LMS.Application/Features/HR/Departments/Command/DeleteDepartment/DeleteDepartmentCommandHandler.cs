using LMS.Application.Abstractions.Loggings;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.Abstractions.UnitOfWorks; // إضافة هذا الاستيراد
using LMS.Common.Enums;
using LMS.Common.Exceptions;
using LMS.Common.Results;
using LMS.Domain.HR.Models;
using MediatR;
using Microsoft.Extensions.Logging; // إضافة هذا الاستيراد للتسجيل

namespace LMS.Application.Features.HR.Departments.Command.DeleteDepartment
{
    public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, Result>
    {
        private readonly ISoftDeletableRepository<Department> _departmentRepo; // تصحيح اسم المتغير
        private readonly IUnitOfWork _unitOfWork; // حقن IUnitOfWork
        private readonly IAppLogger<DeleteDepartmentCommandHandler> _logger; // حقن ILogger

        public DeleteDepartmentCommandHandler(
            ISoftDeletableRepository<Department> departmentRepo, // تصحيح اسم المتغير
            IUnitOfWork unitOfWork, // إضافة IUnitOfWork إلى constructor
            IAppLogger<DeleteDepartmentCommandHandler> logger) // إضافة ILogger إلى constructor
        {
            _departmentRepo = departmentRepo; // تصحيح اسم المتغير
            _unitOfWork = unitOfWork; // تهيئة IUnitOfWork
            _logger = logger; // تهيئة ILogger
        }

        public async Task<Result> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
        {
            // بدء معاملة لضمان ذرية عملية الحذف الناعم
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // استدعاء دالة الحذف الناعم من المستودع.
                // هذه الدالة ستقوم بتحديث خاصية IsDeleted أو ما يعادلها في الكيان وتضع الكيان في حالة Modified.
                await _departmentRepo.SoftDeleteAsync(request.DepartmentId);

                // الالتزام بجميع التغييرات المعلقة (الحذف الناعم للقسم)
                await _unitOfWork.CommitTransactionAsync();

                return Result.Success($"COMMON.{ResponseStatus.DELETE_COMPLETED}");
            }
            catch (EntityNotFoundException ex)
            {
                // التراجع إذا لم يتم العثور على الكيان
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogWarning("Department with ID {DepartmentId} not found for soft deletion.");
                return Result.Failure($"DEPARTMENTS.{ResponseStatus.DEPARTMENT_NOT_FOUNDED}");
            }
            catch (Exception ex)
            {
                // إذا حدث أي خطأ غير متوقع أثناء العملية، نقوم بالتراجع وتسجيل الخطأ
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError("An unexpected error occurred during soft deletion of department with ID {DepartmentId}.", ex);
                return Result.Failure($"COMMON.{ResponseStatus.UNKNOWN_ERROR}");
            }
        }
    }
}