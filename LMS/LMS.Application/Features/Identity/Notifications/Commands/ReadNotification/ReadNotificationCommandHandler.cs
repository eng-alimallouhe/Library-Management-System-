using LMS.Application.Abstractions.Repositories;
using LMS.Application.Abstractions.UnitOfWorks;
using LMS.Common.Enums;
using LMS.Common.Results;
using LMS.Domain.Identity.Models.Notifications;
using MediatR;

namespace LMS.Application.Features.Identity.Notifications.Commands.ReadNotification
{
    public class ReadNotificationCommandHandler : IRequestHandler<ReadNotificationCommand, Result>
    {
        private readonly IBaseRepository<Notification> _notificationRepo;
        private readonly IUnitOfWork _unitOfWork;

        public ReadNotificationCommandHandler(
            IBaseRepository<Notification> notificationRepo,
            IUnitOfWork unitOfWork)
        {
            _notificationRepo = notificationRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(ReadNotificationCommand request, CancellationToken cancellationToken)
        {
            var notification = await _notificationRepo.GetByIdAsync(request.NotificationId);

            if (notification is null)
            {
                return Result.Failure($"COMMON.{ResponseStatus.RECORD_NOT_FOUND}");
            }

            notification.IsRead = true;

            try
            {
                await _unitOfWork.SaveChangesAsync();
                return Result.Success($"COMMON.{ResponseStatus.UPDATE_COMPLETED}");
            } catch (Exception)
            {
                return Result.Failure($"COMMON.{ResponseStatus.STATUS_ALREADY_SET}");
            }
        }
    }
}
