using MediatR;

namespace LMS.Application.Features.Identity.Notifications.Queries.GetUnreadNotificationsCount
{
    public record GetUnreadNotificationsCountQuery(
        Guid UserId) : IRequest<int>;
}
