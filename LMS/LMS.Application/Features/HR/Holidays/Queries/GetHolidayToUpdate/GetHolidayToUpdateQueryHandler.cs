using AutoMapper;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.DTOs.HR;
using LMS.Domain.HR.Models;
using MediatR;

namespace LMS.Application.Features.HR.Holidays.Queries.GetHolidayToUpdate
{
    public class GetHolidayToUpdateQueryHandler : IRequestHandler<GetHolidayToUpdateQuery, HolidayToUpdateDto?>
    {
        private readonly IBaseRepository<Holiday> _holidayRepo;
        private readonly IMapper _mapper;

        public GetHolidayToUpdateQueryHandler(
            IBaseRepository<Holiday> holidayRepo, 
            IMapper mapper)
        {
            _holidayRepo = holidayRepo;
            _mapper = mapper;
        }

        public async Task<HolidayToUpdateDto?> Handle(GetHolidayToUpdateQuery request, CancellationToken cancellationToken)
        {
            var holiday = await _holidayRepo.GetByIdAsync(request.HolidayId);
            if (holiday is null)
                return null;

            return _mapper.Map<HolidayToUpdateDto>(holiday);
        }
    }
}
