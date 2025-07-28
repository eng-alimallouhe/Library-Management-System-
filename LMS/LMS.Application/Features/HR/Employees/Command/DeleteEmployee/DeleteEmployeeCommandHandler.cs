using LMS.Application.Abstractions.Repositories;
using LMS.Common.Enums;
using LMS.Common.Exceptions;
using LMS.Common.Results;
using LMS.Domain.HR.Models;
using MediatR;
using LMS.Application.Abstractions.UnitOfWorks; // إضافة هذا الاستيراد
using LMS.Application.Abstractions.Loggings; // إضافة هذا الاستيراد لواجهة IAppLogger

namespace LMS.Application.Features.HR.Employees.Command.DeleteEmployee
{
    public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, Result>
    {
        private readonly ISoftDeletableRepository<Employee> _employeeRepo;
        private readonly IUnitOfWork _unitOfWork; // حقن IUnitOfWork
        private readonly IAppLogger<DeleteEmployeeCommandHandler> _logger; // حقن IAppLogger

        public DeleteEmployeeCommandHandler(
            ISoftDeletableRepository<Employee> employeeRepo,
            IUnitOfWork unitOfWork, // إضافة IUnitOfWork إلى constructor
            IAppLogger<DeleteEmployeeCommandHandler> logger) // إضافة IAppLogger إلى constructor
        {
            _employeeRepo = employeeRepo;
            _unitOfWork = unitOfWork; // تهيئة IUnitOfWork
            _logger = logger; // تهيئة IAppLogger
        }

        public async Task<Result> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
        {
            // بدء معاملة لضمان ذرية عملية الحذف الناعم
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // 1. استدعاء دالة الحذف الناعم باستخدام EmployeeId الصحيح
                await _employeeRepo.SoftDeleteAsync(request.EmployeeId); // <--- تصحيح: استخدام request.EmployeeId

                // 2. الالتزام بجميع التغييرات المعلقة (الحذف الناعم للموظف)
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation($"Employee with ID {request.EmployeeId} soft-deleted successfully.");
                return Result.Success($"COMMON.{ResponseStatus.TASK_COMPLETED}");
            }
            catch (EntityNotFoundException)
            {
                // التراجع إذا لم يتم العثور على الكيان
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogWarning($"Employee with ID {request.EmployeeId} not found for soft deletion.");
                return Result.Failure($"HR.EMPLOYEES.{ResponseStatus.EMPLOYEE_NOT_FOUNDED}");
            }
            catch (Exception ex)
            {
                // إذا حدث أي خطأ غير متوقع أثناء العملية، نقوم بالتراجع وتسجيل الخطأ
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError($"An unexpected error occurred during soft deletion of employee with ID {request.EmployeeId}.", ex);
                return Result.Failure($"COMMON.{ResponseStatus.UNKNOWN_ERROR}");
            }
        }
    }
}