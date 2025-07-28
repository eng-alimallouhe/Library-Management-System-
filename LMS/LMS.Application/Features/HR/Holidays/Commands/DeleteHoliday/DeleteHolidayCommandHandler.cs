using LMS.Application.Abstractions.Loggings;
using LMS.Application.Abstractions.Repositories;
using LMS.Common.Results;
using LMS.Domain.HR.Models;
using MediatR;

namespace LMS.Application.Features.HR.Holidays.Commands.DeleteHoliday
{
    public class DeleteHolidayCommandHandler : IRequestHandler<DeleteHolidayCommand, Result>
    {
        private readonly IBaseRepository<Holiday> _holidayRepo;
        private readonly IAppLogger<DeleteHolidayCommandHandler> _logger;

        public DeleteHolidayCommandHandler(
            IBaseRepository<Holiday> holidayRepo,
            IAppLogger<DeleteHolidayCommandHandler> logger)
        {
            _holidayRepo = holidayRepo;
            _logger = logger;
        }

        public async Task<Result> Handle(DeleteHolidayCommand request, CancellationToken cancellationToken)
        {
            var holiday = await _holidayRepo.GetByIdAsync(request.HolidayId);
            if (holiday is null)
                return Result.Failure("HR.HOLIDAYS.NOT_FOUND");

            try
            {
                await _holidayRepo.HardDeleteAsync(holiday.HolidayId);
                return Result.Success("COMMON.DELETE_COMPLETED");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while deleting holiday", ex);
                return Result.Failure("COMMON.UNKNOWN_ERROR");
            }
        }
    }
}
