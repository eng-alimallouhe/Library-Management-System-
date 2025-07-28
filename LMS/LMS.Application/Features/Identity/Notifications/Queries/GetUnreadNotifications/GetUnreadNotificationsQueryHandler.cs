using AutoMapper;
using LMS.Application.Abstractions.Identity;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.Identity;
using LMS.Domain.Identity.Models;
using MediatR;

namespace LMS.Application.Features.Identity.Notifications.Queries.GetUnreadNotifications
{
    public class GetUnreadNotificationsQueryHandler : IRequestHandler<GetUnreadNotificationsQuery, PagedResult<NotificationDto>>
    {
        private readonly ISoftDeletableRepository<User> _userRepo;
        private readonly INotificationHelper _notificationHelper;
        private readonly IMapper _mapper;


        public GetUnreadNotificationsQueryHandler(
            ISoftDeletableRepository<User> userRepo,
            INotificationHelper notificationHelper,
            IMapper mapper)
        {
            _userRepo = userRepo;
            _notificationHelper = notificationHelper;
            _mapper = mapper;
        }

        public async Task<PagedResult<NotificationDto>> Handle(GetUnreadNotificationsQuery request, CancellationToken cancellationToken)
        {
            (var items, var count) = await _notificationHelper.GetAllUnreadNotificationAsync(request.UserId, request.PageNumber, request.PageSize);

            var user = await _userRepo.GetByIdAsync(request.UserId);

            var lang = user is null? 0 : (int) user.Language;

            return new PagedResult<NotificationDto>
            {
                Items = _mapper.Map<ICollection<NotificationDto>>(items, 
                context => context.Items["lang"] = lang),
                TotalCount = count,
                CurrentPage = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
