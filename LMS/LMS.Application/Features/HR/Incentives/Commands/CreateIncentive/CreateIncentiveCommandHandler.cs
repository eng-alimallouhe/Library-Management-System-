using AutoMapper;
using LMS.Application.Abstractions;
using LMS.Application.Abstractions.Communications;
using LMS.Application.Abstractions.HR.Events;
using LMS.Application.Abstractions.Loggings;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.DTOs.Identity;
using LMS.Common.Enums;
using LMS.Common.Results;
using LMS.Domain.HR.Models;
using LMS.Domain.Identity.Enums;
using LMS.Domain.Identity.Models.Notifications;
using MediatR;

namespace LMS.Application.Features.HR.Incentives.Commands.CreateIncentive
{
    public class CreateIncentiveCommandHandler : IRequestHandler<CreateIncentiveCommand, Result>
    {
        private readonly ISoftDeletableRepository<Incentive> _incentiveRepo;
        private readonly ISoftDeletableRepository<Employee> _employeeRepo;
        private readonly IFileHostingUploaderHelper _fileUploader;
        private readonly IMapper _mapper;
        private readonly IAppLogger<CreateIncentiveCommandHandler> _logger;
        private readonly IBaseRepository<Notification> _notificationRepo;
        private readonly IBaseRepository<NotificationTranslation> _ntRepo;
        private readonly IPublisher _publisher;

        public CreateIncentiveCommandHandler(
            ISoftDeletableRepository<Incentive> incentiveRepo,
            ISoftDeletableRepository<Employee> employeeRepo,
            IFileHostingUploaderHelper fileUploader,
            IAppLogger<CreateIncentiveCommandHandler> logger,
            IPublisher publisher,
            IBaseRepository<Notification> notificationRepo,
            IBaseRepository<NotificationTranslation> ntRepo,
            IMapper mapper)
        {
            _incentiveRepo = incentiveRepo;
            _employeeRepo = employeeRepo;
            _fileUploader = fileUploader;
            _mapper = mapper;
            _logger = logger;
            _notificationRepo = notificationRepo;
            _ntRepo = ntRepo;
            _publisher = publisher;
        }

        public async Task<Result> Handle(CreateIncentiveCommand request, CancellationToken cancellationToken)
        {
            var employee = await _employeeRepo.GetByIdAsync(request.EmployeeId);

            if (employee is null)
                return Result.Failure($"HR.EMPLOYEES.{ResponseStatus.EMPLOYEE_NOT_FOUNDED}");

            var uploadResult = await _fileUploader.UploadPdfAsync(request.DecisionFile, employee.FullName + Guid.NewGuid());

            if (uploadResult.IsFailed)
                return Result.Failure($"COMMON.{ResponseStatus.SOURCE_NOT_FOUND}");

            var incentive = _mapper.Map<Incentive>(request);
            incentive.DecisionFileUrl = uploadResult.Value!;

            var notification = new Notification
            {
                RedirectUrl = $"https://www.hudacenter.com/hr/incentives/{incentive.IncentiveId}",
            };

            var translations = new List<NotificationTranslation>()
            {
                new()
                {
                    NotificationId = notification.NotificationId,
                    Language = Language.AR,
                    Title = "تم إضافة مكافأة جديدة",
                    Message = "تمت إضافة مكافأة جديدة إلى حسابك، الرجاء الضغط على الرابط لرؤية التفاصيل",
                },
                new()
                {
                    NotificationId = notification.NotificationId,
                    Language = Language.EN,
                    Title = "New Incentive",
                    Message = "A new incentive has been added to your account. Please click the link to view details.",
                }
            };

            notification.Translations = translations;

            try
            {
                await _incentiveRepo.AddAsync(incentive);
                await _notificationRepo.AddAsync(notification);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while adding incentive", ex);
                return Result.Failure($"COMMON.{ResponseStatus.UNKNOWN_ERROR}");
            }

            var translation = translations.FirstOrDefault(nt => nt.Language == employee.Language) ?? translations.First();


            var notificationDto = new NotificationDto
            {
                NotificationId = notification.NotificationId,
                Title = translation.Title,
                Message = translation.Message,
                CreatedAt = notification.CreatedAt.ConvertToSyrianTime(),
                IsRead = notification.IsRead,
                RedirectUrl = notification.RedirectUrl,
            };

            await _publisher.Publish(new IncentiveAddedEvent(employee.UserId, notificationDto), cancellationToken);

            return Result.Success($"COMMON.{ResponseStatus.ADD_COMPLETED}");
        }
    }
}
