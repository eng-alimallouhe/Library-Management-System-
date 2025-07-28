using LMS.Application.DTOs.Identity;

namespace LMS.Application.Abstractions.Identity
{
    public interface INotificationSender
    {
        Task SendNotificationAsync(Guid userId, NotificationDto notification);
        Task SendGlobalNotificationAsync(NotificationDto notification);
    }
}
