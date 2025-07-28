// UpdateLeaveCommandHandler.cs
using AutoMapper;
using LMS.Application.Abstractions.Loggings;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.Abstractions.UnitOfWorks;
using LMS.Common.Enums;
using LMS.Common.Results;
using LMS.Domain.HR.Enums;
using LMS.Domain.HR.Models;
using MediatR;

namespace LMS.Application.Features.HR.Leaves.Commands.UpdateLeave
{
    public class UpdateLeaveCommandHandler : IRequestHandler<UpdateLeaveCommand, Result>
    {
        private readonly ISoftDeletableRepository<Leave> _leaveRepo;
        private readonly IBaseRepository<LeaveBalance> _leaveBalanceRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppLogger<UpdateLeaveCommandHandler> _logger;
        private readonly IMapper _mapper;

        public UpdateLeaveCommandHandler(
            ISoftDeletableRepository<Leave> leaveRepo,
            IBaseRepository<LeaveBalance> leaveBalanceRepo,
            IUnitOfWork unitOfWork,
            IAppLogger<UpdateLeaveCommandHandler> logger,
            IMapper mapper)
        {
            _leaveRepo = leaveRepo;
            _leaveBalanceRepo = leaveBalanceRepo;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result> Handle(UpdateLeaveCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var leave = await _leaveRepo.GetByIdAsync(request.LeaveId);

                if (leave is null || !leave.IsActive)
                    return Result.Failure($"COMMON.{ResponseStatus.RECORD_NOT_FOUND}");

                if (leave.LeaveStatus != LeaveStatus.Pending)
                {
                    return Result.Failure($"COMMON.{ResponseStatus.RECORD_STATUS_CONNOT_BE_MODIFIED}");
                }

                if (leave.EmployeeId != request.EmployeeId)
                    return Result.Failure($"COMMON.{ResponseStatus.AUTHORIZATION_REQUIERD}");

                var balance = await _leaveBalanceRepo.GetByExpressionAsync(lb => lb.EmployeeId == leave.EmployeeId, true);
                
                if (balance is null)
                    return Result.Failure($"HR.LEAVES.{ResponseStatus.BALANCE_NOT_FOUNDED}");

                
                var oldDays = (leave.EndDate - leave.StartDate).Days + 1;

                
                balance.UsedBalance -= oldDays;

                
                var newDays = (request.EndDate - request.StartDate).Days + 1;

                if (newDays > balance.RemainingBalance)
                    return Result.Failure($"HR.EMPLOYEES.{ResponseStatus.BALANCE_NOT_ENOUGH}");

                
                _mapper.Map(request, leave);
                
                leave.IsPaid = leave.LeaveType == LeaveType.Unpaid? false : true;
                
                balance.UsedBalance += newDays;


                await _unitOfWork.CommitTransactionAsync();
                return Result.Success($"COMMON.{ResponseStatus.UPDATE_COMPLETED}");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError("Error while updating leave", ex);
                return Result.Failure($"COMMON.{ResponseStatus.UNKNOWN_ERROR}");
            }
        }
    }
}