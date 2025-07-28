using AutoMapper;
using LMS.Application.Abstractions.Loggings;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.Abstractions.UnitOfWorks;
using LMS.Common.Results;
using LMS.Domain.HR.Models;
using MediatR;

namespace LMS.Application.Features.HR.Holidays.Commands.UpdateHoliday
{
    public class UpdateHolidayCommandHandler : IRequestHandler<UpdateHolidayCommand, Result>
    {
        private readonly IBaseRepository<Holiday> _holidayRepo;
        private readonly IAppLogger<UpdateHolidayCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateHolidayCommandHandler(
            IBaseRepository<Holiday> holidayRepo,
            IAppLogger<UpdateHolidayCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _holidayRepo = holidayRepo;
            _logger = logger;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateHolidayCommand request, CancellationToken cancellationToken)
        {
            var holiday = await _holidayRepo.GetByIdAsync(request.HolidayId);
            if (holiday is null)
                return Result.Failure("HR.HOLIDAYS.NOT_FOUND");

            
            _mapper.Map(request, holiday);

            try
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success("COMMON.UPDATE_COMPLETED");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while updating holiday", ex);
                return Result.Failure("COMMON.UNKNOWN_ERROR");
            }
        }
    }
}
