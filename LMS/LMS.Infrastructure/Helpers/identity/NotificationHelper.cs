using System.Linq.Expressions;
using LMS.Application.Abstractions.Identity;
using LMS.Application.Abstractions.Repositories;
using LMS.Domain.Abstractions.Specifications;
using LMS.Domain.Identity.Models.Notifications;

namespace LMS.Infrastructure.Helpers.identity
{
    public class NotificationHelper : INotificationHelper
    {
        private readonly IBaseRepository<Notification> _notificationRepo;

        public NotificationHelper(IBaseRepository<Notification> notificationRepo)
        {
            _notificationRepo = notificationRepo;
        }

        public async Task<(ICollection<Notification> items, int count)> GetAllUnreadNotificationAsync(Guid userId, int pageNumber, int pageSize)
        {
            return await _notificationRepo.GetAllAsync(new UnreadNotificationSpecification(userId, pageNumber, pageSize));
        }
    }


    public class UnreadNotificationSpecification : ISpecification<Notification>
    {
        private readonly Guid _userId;
        private readonly int _pageNumber;
        private readonly int _pageSize;


        public UnreadNotificationSpecification(
            Guid userId,
            int pageNumber,
            int pageSize)
        {
            _userId = userId;
            _pageNumber = pageNumber;
            _pageSize = pageSize;
        }


        public Expression<Func<Notification, bool>>? Criteria => 
            n => n.UserId == _userId && !n.IsRead;
        
        public List<string> Includes => [nameof(Notification.Translations)];

        public bool IsTrackingEnabled => false;

        public Expression<Func<Notification, object>>? OrderBy => n => n.CreatedAt;

        public Expression<Func<Notification, object>>? OrderByDescending => null;

        public int? Skip => _pageNumber;

        public int? Take => _pageSize;
    }
}
