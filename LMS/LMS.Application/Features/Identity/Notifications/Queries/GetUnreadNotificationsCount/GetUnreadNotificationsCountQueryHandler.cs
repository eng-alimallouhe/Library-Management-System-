using LMS.Application.Abstractions.Repositories;
using LMS.Domain.Identity.Models.Notifications;
using MediatR;

namespace LMS.Application.Features.Identity.Notifications.Queries.GetUnreadNotificationsCount
{
    public class GetUnreadNotificationsCountQueryHandler : IRequestHandler<GetUnreadNotificationsCountQuery, int>
    {
        private readonly IBaseRepository<Notification> _notificationRepo;

        public GetUnreadNotificationsCountQueryHandler(IBaseRepository<Notification> notificationRepo)
        {
            _notificationRepo = notificationRepo;
        }

        public async Task<int> Handle(GetUnreadNotificationsCountQuery request, CancellationToken cancellationToken)
        {
            return await _notificationRepo.CountAsync(n => 
                n.UserId == request.UserId && !n.IsRead
            );
        }
    }
}
