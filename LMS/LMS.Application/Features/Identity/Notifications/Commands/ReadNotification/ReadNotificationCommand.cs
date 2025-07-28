using LMS.Common.Results;
using MediatR;

namespace LMS.Application.Features.Identity.Notifications.Commands.ReadNotification
{
    public class ReadNotificationCommand : IRequest<Result>
    {
        public Guid NotificationId { get; set; }
    }
}
