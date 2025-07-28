using LMS.Common.Enums;
using LMS.Common.Results;
using LMS.Domain.HR.Enums;
using LMS.Domain.HR.Models;
using LMS.Application.Abstractions.Loggings;
using MediatR;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.Abstractions.UnitOfWorks;

namespace LMS.Application.Features.HR.Leaves.Commands.DeleteLeave
{
    public class DeleteLeaveCommandHandler : IRequestHandler<DeleteLeaveCommand, Result>
    {
        private readonly ISoftDeletableRepository<Leave> _leaveRepo;
        private readonly IBaseRepository<LeaveBalance> _balanceRepo;
        private readonly IAppLogger<DeleteLeaveCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteLeaveCommandHandler(
            ISoftDeletableRepository<Leave> leaveRepo,
            IBaseRepository<LeaveBalance> balanceRepo,
            IAppLogger<DeleteLeaveCommandHandler> logger,
            IUnitOfWork unitOfWork)
        {
            _leaveRepo = leaveRepo;
            _balanceRepo = balanceRepo;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteLeaveCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var leave = await _leaveRepo.GetByIdAsync(request.LeaveId);

                if (leave is null || !leave.IsActive)
                    return Result.Failure($"COMMON.{ResponseStatus.RECORD_NOT_FOUND}");

                if (leave.EmployeeId != request.RequestingUserId)
                    return Result.Failure($"COMMON.{ResponseStatus.AUTHORIZATION_REQUIERD}");

                if (leave.LeaveStatus != LeaveStatus.Pending)
                    return Result.Failure($"COMMON.{ResponseStatus.UNABLE_DELETE_ELEMENT}");

                var duration = (leave.EndDate - leave.StartDate).Days;

                var balance = await _balanceRepo.GetByExpressionAsync(b => b.EmployeeId == leave.EmployeeId, true);

                if (balance is not null)
                {
                    if (balance.UsedBalance == 0)
                    {
                        balance.CarriedOverBalance += duration;
                    }
                    else
                    {
                        balance.UsedBalance -= duration;
                    }
                }

                await _leaveRepo.SoftDeleteAsync(leave.LeaveId);
                await _unitOfWork.CommitTransactionAsync();

                return Result.Success($"COMMON.{ResponseStatus.DELETE_COMPLETED}");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while deleting leave", ex);
                await _unitOfWork.RollbackTransactionAsync();
                return Result.Failure($"COMMON.{ResponseStatus.UNKNOWN_ERROR}");
            }
        }
    }
}
