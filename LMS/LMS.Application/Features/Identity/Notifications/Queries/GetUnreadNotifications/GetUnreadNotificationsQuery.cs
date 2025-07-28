using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.Identity;
using MediatR;

namespace LMS.Application.Features.Identity.Notifications.Queries.GetUnreadNotifications
{
    public record GetUnreadNotificationsQuery(
        Guid UserId,
        int PageNumber = 1,
        int PageSize = 10) : IRequest<PagedResult<NotificationDto>>;
}
