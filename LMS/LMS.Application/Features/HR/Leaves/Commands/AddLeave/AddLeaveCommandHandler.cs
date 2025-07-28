using AutoMapper;
using LMS.Application.Abstractions;
using LMS.Application.Abstractions.HR;
using LMS.Application.Abstractions.HR.Events;
using LMS.Application.Abstractions.Loggings;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.Abstractions.UnitOfWorks;
using LMS.Application.DTOs.Identity;
using LMS.Common.Enums;
using LMS.Common.Results;
using LMS.Domain.HR.Enums;
using LMS.Domain.HR.Models;
using LMS.Domain.Identity.Enums;
using LMS.Domain.Identity.Models;
using LMS.Domain.Identity.Models.Notifications;
using MediatR;

namespace LMS.Application.Features.HR.Leaves.Commands.AddLeave
{
    public class AddLeaveCommandHandler : IRequestHandler<AddLeaveCommand, Result>
    {
        private readonly ISoftDeletableRepository<Leave> _leaveRepo;
        private readonly IBaseRepository<LeaveBalance> _leaveBalanceRepo;
        private readonly ISoftDeletableRepository<Employee> _employeeRepo;
        private readonly IBaseRepository<Notification> _notificationRepo;
        private readonly IBaseRepository<NotificationTranslation> _ntRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppLogger<AddLeaveCommandHandler> _logger;
        private readonly IEmployeeHelper _employeeHelper;
        private readonly IMapper _mapper;
        private readonly IPublisher _publisher;

        public AddLeaveCommandHandler(
            ISoftDeletableRepository<Leave> leaveRepo,
            IBaseRepository<LeaveBalance> leaveBalanceRepo,
            ISoftDeletableRepository<Employee> employeeRepo,
            IBaseRepository<Notification> notificationRepo,
            IBaseRepository<NotificationTranslation> ntRepo,
            IAppLogger<AddLeaveCommandHandler> logger,
            IUnitOfWork unitOfWork,
            IEmployeeHelper employeeHelper,
            IMapper mapper,
            IPublisher publisher)
        {
            _leaveRepo = leaveRepo;
            _leaveBalanceRepo = leaveBalanceRepo;
            _employeeRepo = employeeRepo;
            _notificationRepo = notificationRepo;
            _ntRepo = ntRepo;
            _logger = logger;
            _employeeHelper = employeeHelper;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _publisher = publisher;
        }

        public async Task<Result> Handle(AddLeaveCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();

            var employee = await _employeeRepo.GetByIdAsync(request.EmployeeId);
            if (employee is null)
                return Result.Failure($"HR.EMPLOYEES.{ResponseStatus.EMPLOYEE_NOT_FOUNDED}");

            var balance = await _leaveBalanceRepo.GetByExpressionAsync(lb => lb.EmployeeId == employee.UserId, true);

            if (balance is null)
            {
                return Result.Failure($"HR.EMPLOYEES.{ResponseStatus.BALANCE_NOT_FOUNDED}");
            }

            var range = (request.EndDate - request.StartDate).Days;

            if (range < 0) return Result.Failure($"HR.LEAVES.{ResponseStatus.INVALID_DATE_RANGE}");

            if (range > balance.RemainingBalance && request.LeaveType != (int) LeaveType.Unpaid)
            {
                return Result.Failure($"HR.EMPLOYEES.{ResponseStatus.BALANCE_NOT_ENOUGH}");
            }

            var leave = _mapper.Map<Leave>(request);
            leave.IsPaid = request.LeaveType != (int)LeaveType.Unpaid;

            try
            {
                await _leaveRepo.AddAsync(leave);

                var managerUserIds = await _employeeHelper.GetManagersIdsAsync();

                var redirectUrl = $"https://www.hudacenter.com/hr/leaves/{leave.LeaveId}";

                var notificationDto = new NotificationDto
                {
                    NotificationId = Guid.NewGuid(),
                    Title = "HR.LEAVES.NEW_LEAVE_REQUEST_TITLE",
                    Message = "HR.LEAVES.NEW_LEAVE_REQUEST_MESSAGE",
                    CreatedAt = DateTime.UtcNow.ConvertToSyrianTime(),
                    IsRead = false,
                    RedirectUrl = redirectUrl,
                };

                var notifications = new List<Notification>();


                foreach (var id in managerUserIds)
                {
                    notifications.Add(new Notification
                    {
                        UserId = id,
                        RedirectUrl = redirectUrl,
                        Translations = new List<NotificationTranslation>
                        {
                            new NotificationTranslation
                            {
                                Language = Language.AR,
                                Title = "طلب إجازة جديد",
                                Message = $"تم إرسال طلب إجازة جديد من الموظف {employee.FullName}",
                            },
                            new NotificationTranslation
                            {
                                Language = Language.EN,
                                Title = "New Leave Request",
                                Message = $"A new leave request has been submitted by {employee.FullName}",
                            }
                        }
                    });
                }

                const int batchSize = 100;
                foreach (var batch in notifications.Chunk(batchSize))
                {
                    await _notificationRepo.AddRangeAsync(batch.ToList());
                }


                balance.UsedBalance += range;

                await _publisher.Publish(new LeaveRequestedEvent(managerUserIds.ToList(), notificationDto), cancellationToken);

                await _unitOfWork.CommitTransactionAsync();

                return Result.Success($"COMMON.{ResponseStatus.ADD_COMPLETED}");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while adding leave", ex);
                await _unitOfWork.RollbackTransactionAsync();
                return Result.Failure($"COMMON.{ResponseStatus.UNKNOWN_ERROR}");
            }
        }
    }
}