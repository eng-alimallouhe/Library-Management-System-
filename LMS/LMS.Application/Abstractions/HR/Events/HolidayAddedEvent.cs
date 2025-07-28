using LMS.Application.DTOs.Identity;
using MediatR;

namespace LMS.Application.Abstractions.HR.Events
{
    public class HolidayAddedEvent : INotification
    {
        public NotificationDto NotificationDetails { get; }

        public HolidayAddedEvent(NotificationDto notificationDetails)
        {
            NotificationDetails = notificationDetails;
        }
    }
}