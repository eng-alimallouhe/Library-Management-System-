using LMS.Application.Abstractions;
using LMS.Application.Abstractions.HR;
using LMS.Application.Abstractions.HR.Events;
using LMS.Application.Abstractions.Loggings;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.Abstractions.UnitOfWorks;
using LMS.Application.DTOs.Identity;
using LMS.Application.Settings;
using LMS.Common.Enums;
using LMS.Common.Results;
using LMS.Domain.HR.Enums;
using LMS.Domain.HR.Models;
using LMS.Domain.Identity.Enums;
using LMS.Domain.Identity.Models.Notifications;
using MediatR;
using Microsoft.Extensions.Options;

namespace LMS.Application.Features.HR.Leaves.Commands.UpdateLeaveStatus
{
    public class UpdateLeaveStatusCommandHandler : IRequestHandler<UpdateLeaveStatusCommand, Result>
    {
        private readonly ISoftDeletableRepository<Leave> _leaveRepo;
        private readonly IBaseRepository<LeaveBalance> _leaveBalanceRepo;
        private readonly ISoftDeletableRepository<Employee> _employeeRepo;
        private readonly IEmployeeHelper _employeeHelper;
        private readonly IBaseRepository<Notification> _notificationRepo;
        private readonly IBaseRepository<NotificationTranslation> _ntRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppLogger<UpdateLeaveStatusCommandHandler> _logger;
        private readonly IPublisher _publisher;
        private readonly FrontendSettings _frontend;


        public UpdateLeaveStatusCommandHandler(
            ISoftDeletableRepository<Leave> leaveRepo,
            IBaseRepository<LeaveBalance> leaveBalanceRepo,
            ISoftDeletableRepository<Employee> employeeRepo,
            IEmployeeHelper employeeHelper,
            IBaseRepository<Notification> notificationRepo,
            IBaseRepository<NotificationTranslation> ntRepo,
            IUnitOfWork unitOfWork,
            IAppLogger<UpdateLeaveStatusCommandHandler> logger,
            IOptions<FrontendSettings> frontendSettings,
            IPublisher publisher)
        {
            _leaveRepo = leaveRepo;
            _leaveBalanceRepo = leaveBalanceRepo;
            _employeeRepo = employeeRepo;
            _employeeHelper = employeeHelper;
            _notificationRepo = notificationRepo;
            _ntRepo = ntRepo;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _publisher = publisher;
            _frontend = frontendSettings.Value;
        }

        public async Task<Result> Handle(UpdateLeaveStatusCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var leave = await _leaveRepo.GetByIdAsync(request.LeaveId);
                if (leave is null)
                    return Result.Failure("HR.LEAVES.NOT_FOUND");

                var managers = await _employeeHelper.GetManagersIdsAsync();
                if (!managers.Contains(request.UpdatedBy))
                    return Result.Failure($"COMMON.{ResponseStatus.AUTHORIZATION_REQUIERD}");

                if ((int)leave.LeaveStatus == request.NewStatus)
                    return Result.Success($"COMMON.{ResponseStatus.STATUS_ALREADY_SET}");

                var employee = await _employeeRepo.GetByIdAsync(leave.EmployeeId);
                if (employee is null)
                    return Result.Failure($"HR.EMPLOYEES.{ResponseStatus.EMPLOYEE_NOT_FOUNDED}");

                var balance = await _leaveBalanceRepo.GetByExpressionAsync(lb => lb.EmployeeId == employee.UserId, true);
                if (balance is null)
                    return Result.Failure($"HR.LEAVES.{ResponseStatus.BALANCE_NOT_FOUNDED}");

                var days = (leave.EndDate - leave.StartDate).Days;


                if (request.NewStatus == (int)LeaveStatus.Rejected && leave.IsPaid)
                {
                    if (balance.UsedBalance == 0)
                        balance.CarriedOverBalance += days;
                    else
                        balance.UsedBalance -= days;

                }

                leave.LeaveStatus = (LeaveStatus)request.NewStatus;
                leave.UpdatedAt = DateTime.UtcNow;



                var notification = new Notification
                {
                    RedirectUrl = $"{_frontend.BaseUrl}/hr/leaves/{leave.LeaveId}",
                };

                var translations = new List<NotificationTranslation>
            {
                new NotificationTranslation
                {
                    NotificationId = notification.NotificationId,
                    Language = Language.AR,
                    Title = "تحديث حالة الإجازة",
                    Message = $"تم تحديث حالة الإجازة الخاصة بك إلى {GetStatusString((LeaveStatus)request.NewStatus, Language.AR)}"
                },
                new NotificationTranslation
                {
                    NotificationId = notification.NotificationId,
                    Language = Language.EN,
                    Title = "Leave Status Updated",
                    Message = $"Your leave request status has been updated to {GetStatusString((LeaveStatus)request.NewStatus, Language.EN)}"
                }
            };

                await _notificationRepo.AddAsync(notification);
                await _unitOfWork.SaveChangesAsync();

                await _ntRepo.AddRangeAsync(translations);
                await _unitOfWork.SaveChangesAsync();

                var translation = translations.FirstOrDefault(t => t.Language == employee.Language) ?? translations.First();

                var dto = new NotificationDto
                {
                    NotificationId = notification.NotificationId,
                    Title = translation.Title,
                    Message = translation.Message,
                    CreatedAt = notification.CreatedAt.ConvertToSyrianTime(),
                    IsRead = false,
                    RedirectUrl = notification.RedirectUrl
                };

                await _publisher.Publish(new LeaveStatusUpdatedEvent(employee.UserId, dto), cancellationToken);

                await _unitOfWork.CommitTransactionAsync();

                return Result.Success($"COMMON.{ResponseStatus.UPDATE_COMPLETED}");
            }
            catch
            {
                return Result.Failure($"COMMON.{ResponseStatus.UNKNOWN_ERROR}");
            }
        }

        private string GetStatusString(LeaveStatus status, Language lang)
        {
            return status switch
            {
                LeaveStatus.Approved => lang == Language.AR ? "مقبولة" : "Approved",
                LeaveStatus.Rejected => lang == Language.AR ? "مرفوضة" : "Rejected",
                LeaveStatus.Pending => lang == Language.AR ? "قيد الانتظار" : "Pending",
                _ => lang == Language.AR ? "غير معروفة" : "Unknown"
            };
        }
    }

}
