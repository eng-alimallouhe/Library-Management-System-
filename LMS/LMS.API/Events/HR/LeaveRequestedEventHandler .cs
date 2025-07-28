using LMS.API.Hubs.Notifications;
using LMS.Application.Abstractions.HR.Events;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace LMS.API.Events.HR
{
    public class LeaveRequestedEventHandler : INotificationHandler<LeaveRequestedEvent>
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public LeaveRequestedEventHandler(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Handle(LeaveRequestedEvent notificationEvent, CancellationToken cancellationToken)
        {
            foreach (var managerId in notificationEvent.ManagerUserIds)
            {
                await _hubContext.Clients.User(managerId.ToString())
                                 .SendAsync("ReceiveNotification", notificationEvent.NotificationDetails, cancellationToken);
            }
        }
    }
}
