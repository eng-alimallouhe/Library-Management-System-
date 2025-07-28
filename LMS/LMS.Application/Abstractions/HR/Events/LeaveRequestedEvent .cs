// LMS.Application/Abstractions/HR/Events/LeaveRequestedEvent.cs
using LMS.Application.DTOs.Identity;
using MediatR;

namespace LMS.Application.Abstractions.HR.Events
{
    public class LeaveRequestedEvent : INotification
    {
        public NotificationDto NotificationDetails { get; }
        public List<Guid> ManagerUserIds { get; }

        public LeaveRequestedEvent(List<Guid> managerUserIds, NotificationDto notificationDetails)
        {
            ManagerUserIds = managerUserIds;
            NotificationDetails = notificationDetails;
        }
    }
}
