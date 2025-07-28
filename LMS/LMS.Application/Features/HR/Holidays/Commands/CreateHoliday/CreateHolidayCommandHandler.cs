using AutoMapper;
using LMS.Application.Abstractions;
using LMS.Application.Abstractions.HR.Events;
using LMS.Application.Abstractions.Loggings;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.DTOs.Identity;
using LMS.Application.Settings;
using LMS.Common.Enums;
using LMS.Common.Results;
using LMS.Domain.HR.Models;
using LMS.Domain.Identity.Enums;
using LMS.Domain.Identity.Models;
using LMS.Domain.Identity.Models.Notifications;
using MediatR;
using Microsoft.Extensions.Options;

namespace LMS.Application.Features.HR.Holidays.Commands.CreateHoliday
{
    public class CreateHolidayCommandHandler : IRequestHandler<CreateHolidayCommand, Result>
    {
        private readonly IBaseRepository<Holiday> _holidayRepo;
        private readonly IBaseRepository<Notification> _notificationRepo;
        private readonly IAppLogger<CreateHolidayCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IPublisher _publisher;
        private readonly ISoftDeletableRepository<User> _userRepo;
        private readonly FrontendSettings _settings;

        public CreateHolidayCommandHandler(
            IBaseRepository<Holiday> holidayRepo,
            IAppLogger<CreateHolidayCommandHandler> logger,
            IBaseRepository<Notification> notificationRepo,
            IPublisher publisher,
            IOptions<FrontendSettings> options,
            ISoftDeletableRepository<User> userRepo,
            IMapper mapper)
        {
            _holidayRepo = holidayRepo;
            _logger = logger;
            _mapper = mapper;
            _settings = options.Value;
            _notificationRepo = notificationRepo;
            _publisher = publisher;
            _userRepo = userRepo;
        }

        public async Task<Result> Handle(CreateHolidayCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var holiday = _mapper.Map<Holiday>(request);

                await _holidayRepo.AddAsync(holiday);

                
                var frontBaseUrl = _settings.BaseUrl.TrimEnd('/');
                var redirectUrl = $"{frontBaseUrl}/hr/holidays/{holiday.HolidayId}";


                var users = await _userRepo.GetAllByExpressionAsync(u => !u.IsDeleted && !u.IsLocked);

                ICollection<Notification> notifications = new List<Notification>();


                foreach (var item in users)
                {
                    notifications.Add(new Notification
                    {
                        UserId = item.UserId,
                        RedirectUrl = redirectUrl,
                        Translations = new List<NotificationTranslation>
                        {
                            new NotificationTranslation
                            {
                                Language = Language.AR,
                                Title = "عطلة جديدة",
                                Message = $"تمت إضافة عطلة جديدة: {holiday.Name}"
                            },
                            new NotificationTranslation
                            {
                                Language = Language.EN,
                                Title = "New Holiday",
                                Message = $"A new holiday has been added"
                            }
                        }
                    });
                }

                const int batchSize = 100;
                foreach (var batch in notifications.Chunk(batchSize))
                {
                    await _notificationRepo.AddRangeAsync(batch.ToList());
                }


                // نشر الحدث
                var dto = new NotificationDto
                {
                    NotificationId = Guid.NewGuid(),
                    Title = "NOTIFICATIONS.HOLIDAY.TITLE",
                    Message = "NOTIFICATIONS.HOLIDAY.MESSAGE",
                    CreatedAt = DateTime.UtcNow.ConvertToSyrianTime(),
                    RedirectUrl = redirectUrl,
                    IsRead = false
                };

                await _publisher.Publish(new HolidayAddedEvent(dto), cancellationToken);


                return Result.Success($"COMMON.{ResponseStatus.ADD_COMPLETED}");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while adding holiday", ex);
                return Result.Failure("COMMON.UNKNOWN_ERROR");
            }
        }
    }
}
