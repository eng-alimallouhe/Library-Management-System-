using LMS.API.Hubs.Notifications;
using LMS.Application.Abstractions.HR.Events;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace LMS.API.Events.HR
{
    public class HolidayCreatedEventHandler : INotificationHandler<HolidayAddedEvent>
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public HolidayCreatedEventHandler(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Handle(HolidayAddedEvent notification, CancellationToken cancellationToken)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", notification.NotificationDetails);
        }
    }
}