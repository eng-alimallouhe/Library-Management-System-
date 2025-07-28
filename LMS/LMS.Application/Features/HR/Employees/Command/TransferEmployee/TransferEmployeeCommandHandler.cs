using AutoMapper;
using LMS.Application.Abstractions.HR;
using LMS.Common.Results;
using MediatR;
using LMS.Application.Abstractions.UnitOfWorks;
using LMS.Application.Abstractions.Loggings;
using LMS.Common.Enums; // تأكد من وجود هذا الاستيراد لواجهة IAppLogger

namespace LMS.Application.Features.HR.Employees.Command.TransferEmployee
{
    public class TransferEmployeeCommandHandler : IRequestHandler<TransferEmployeeCommand, Result>
    {
        private readonly IEmployeeHelper _employeeHelper;
        private readonly IMapper _mapper;
        private readonly IAppLogger<TransferEmployeeCommandHandler> _logger; // استخدام IAppLogger<T>
        private readonly IUnitOfWork _unitOfWork;

        public TransferEmployeeCommandHandler(
            IEmployeeHelper employeeHelper,
            IMapper mapper,
            IAppLogger<TransferEmployeeCommandHandler> logger, // حقن IAppLogger
            IUnitOfWork unitOfWork)
        {
            _employeeHelper = employeeHelper;
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(TransferEmployeeCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var result = await _employeeHelper.TransferEmployeeAsync(request.EmployeeId, request.DepartmentId, request.AppointmentDecision);

                if (result.IsFailed)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    // استخدام LogWarning لأن الفشل قد لا يكون استثناءً
                    _logger.LogWarning("Employee transfer logic failed for EmployeeId: {request.EmployeeId}, DepartmentId: {request.DepartmentId}. Status: {result.StatusKey}");
                    return result;
                }

                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("Employee {request.EmployeeId} successfully transferred to department {request.DepartmentId}.");
                return Result.Success($"COMMON.{ResponseStatus.TASK_COMPLETED}");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                // تعديل استخدام LogError هنا: رسالة ثم الاستثناء
                _logger.LogError($"An unexpected error occurred during employee transfer for EmployeeId: {request.EmployeeId}, DepartmentId: {request.DepartmentId}", ex);
                return Result.Failure($"COMMON.{ResponseStatus.UNKNOWN_ERROR}");
            }
        }
    }
}