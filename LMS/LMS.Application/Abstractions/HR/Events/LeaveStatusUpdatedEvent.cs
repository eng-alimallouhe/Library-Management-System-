using LMS.Application.DTOs.Identity;
using MediatR;

namespace LMS.Application.Abstractions.HR.Events
{
    public class LeaveStatusUpdatedEvent : INotification
    {
        public Guid EmployeeId { get; }
        public NotificationDto NotificationDetails { get; }

        public LeaveStatusUpdatedEvent(Guid employeeId, NotificationDto notificationDetails)
        {
            EmployeeId = employeeId;
            NotificationDetails = notificationDetails;
        }
    }
}
