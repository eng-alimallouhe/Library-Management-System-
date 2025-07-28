using AutoMapper;
using LMS.Application.Abstractions;
using LMS.Application.Abstractions.Communications;
using LMS.Application.Abstractions.HR.Events;
using LMS.Application.Abstractions.Loggings;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.Abstractions.UnitOfWorks;
using LMS.Application.DTOs.Identity;
using LMS.Common.Enums;
using LMS.Common.Results;
using LMS.Domain.HR.Models;
using LMS.Domain.Identity.Enums;
using LMS.Domain.Identity.Models.Notifications;
using MediatR;

namespace LMS.Application.Features.HR.Penalties.Commands.AddPenalty
{
    public class AddPenaltyCommandHandler : IRequestHandler<AddPenaltyCommand, Result>
    {
        private readonly ISoftDeletableRepository<Penalty> _penaltyRepo;
        private readonly ISoftDeletableRepository<Employee> _employeeRepo;
        private readonly IFileHostingUploaderHelper _fileHostingUploaderHelper;
        private readonly IMapper _mapper;
        private readonly IAppLogger<AddPenaltyCommandHandler> _appLogger;
        private readonly IBaseRepository<Notification> _notificationRepo;
        private readonly IBaseRepository<NotificationTranslation> _ntRepo;
        private readonly IPublisher _publisher;
        private readonly IUnitOfWork _unitOfWork;

        public AddPenaltyCommandHandler(
            ISoftDeletableRepository<Penalty> penaltyRepo,
            ISoftDeletableRepository<Employee> employeeRepo,
            IFileHostingUploaderHelper fileHostingUploaderHelper,
            IAppLogger<AddPenaltyCommandHandler> appLogger,
            IPublisher publisher,
            IBaseRepository<Notification> notificationRepo,
            IBaseRepository<NotificationTranslation> ntRepo,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _penaltyRepo = penaltyRepo;
            _employeeRepo = employeeRepo;
            _fileHostingUploaderHelper = fileHostingUploaderHelper;
            _mapper = mapper;
            _appLogger = appLogger;
            _unitOfWork = unitOfWork;
            _notificationRepo = notificationRepo;
            _ntRepo = ntRepo;
            _publisher = publisher;
        }

        public async Task<Result> Handle(AddPenaltyCommand request, CancellationToken cancellationToken)
        {
            var employee = await _employeeRepo.GetByIdAsync(request.EmployeeId);

            if (employee is null)
            {
                return Result.Failure($"HR.EMPLOYEES.{ResponseStatus.EMPLOYEE_NOT_FOUNDED}");
            }

            var uploadReult = await _fileHostingUploaderHelper.UploadPdfAsync(request.DecisionFile, null);

            if (uploadReult.IsFailed)
            {
                return Result.Failure($"COMMON.{ResponseStatus.SOURCE_NOT_FOUND}");
            }

            var penalty = _mapper.Map<Penalty>(request);

            penalty.DecisionFileUrl = uploadReult.Value!;

            var notification = new Notification
            {
                RedirectUrl = $"https://www.hudacenter.com/hr/penalties/{penalty.PenaltyId}",
                UserId = request.EmployeeId
            };

            var translations = new List<NotificationTranslation>() {
                new NotificationTranslation
                {
                    NotificationId = notification.NotificationId,
                    Language = Language.AR,
                    Title = "تم إضافة عقوبة جديدة",
                    Message = "تمت إضافة عقوبة جديدة إلى حسابك, الرجاء الضغط على الرابط لرؤية التفاصيل",
                },
                new NotificationTranslation
                {
                    NotificationId = notification.NotificationId,
                    Language = Language.EN,
                    Title = "New Penalty",
                    Message = "A new penalty has been added to your account. Please click on the link to view the details.",
                }
            };

            try
            {
                await _penaltyRepo.AddAsync(penalty);
                await _notificationRepo.AddAsync(notification);
                await _unitOfWork.SaveChangesAsync();
                await _ntRepo.AddRangeAsync(translations);
                await _unitOfWork.SaveChangesAsync();
                
            } catch (Exception ex)
            {
                _appLogger.LogError($"Error While Adding a penalty : {ex.Message}", ex);
                return Result.Failure($"COMMON.{ResponseStatus.UNKNOWN_ERROR}");
            }

            var translation = translations.FirstOrDefault(nt => nt.Language == employee.Language);

            if (translation is null)
            {
                translation = new NotificationTranslation();
            }

            var notificationDto = new NotificationDto
            {
                NotificationId = notification.NotificationId,
                Title = translation.Title,
                Message = translation.Message,
                CreatedAt = notification.CreatedAt.ConvertToSyrianTime(),
                RedirectUrl = notification.RedirectUrl,
            };

            await _publisher.Publish(new PenaltyAddedEvent(employee.UserId, notificationDto), cancellationToken);

            return Result.Success($"COMMON.{ResponseStatus.ADD_COMPLETED}");
        }
    }
}
