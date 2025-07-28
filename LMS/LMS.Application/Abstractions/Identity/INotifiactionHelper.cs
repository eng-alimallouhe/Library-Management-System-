using LMS.Domain.Identity.Models.Notifications;

namespace LMS.Application.Abstractions.Identity
{
    public interface INotificationHelper
    {
        Task<(ICollection<Notification> items, int count)> GetAllUnreadNotificationAsync(Guid userId, int pageNumber, int pageSize);
    }
}
