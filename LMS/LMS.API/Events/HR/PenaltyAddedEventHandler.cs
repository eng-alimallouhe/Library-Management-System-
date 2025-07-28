using LMS.API.Hubs.Notifications;
using LMS.Application.Abstractions.HR.Events;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace LMS.API.Events.HR
{
    public class PenaltyAddedEventHandler : INotificationHandler<PenaltyAddedEvent>
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public PenaltyAddedEventHandler(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Handle(PenaltyAddedEvent notificationEvent, CancellationToken cancellationToken)
        {
            await _hubContext.Clients.User(notificationEvent.EmployeeId.ToString())
                             .SendAsync("ReceiveNotification", notificationEvent.NotificationDetails, cancellationToken);
        }
    }
}
