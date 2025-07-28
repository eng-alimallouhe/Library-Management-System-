using LMS.Application.DTOs.Identity;
using MediatR;

namespace LMS.Application.Abstractions.HR.Events
{
    public class PenaltyAddedEvent : INotification
    {
        public Guid EmployeeId { get; }
        public NotificationDto NotificationDetails { get; }

        public PenaltyAddedEvent(Guid employeeId, NotificationDto notificationDetails)
        {
            EmployeeId = employeeId;
            NotificationDetails = notificationDetails;
        }
    }
}
