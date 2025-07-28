// LMS.Application.Features.HR.Holidays.Queries.GetAllHolidays.GetAllHolidaysQueryHandler.cs

using AutoMapper;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.DTOs.HR;
using LMS.Domain.HR.Models;
using MediatR;

namespace LMS.Application.Features.HR.Holidays.Queries.GetAllHolidays
{
    public class GetAllHolidaysQueryHandler : IRequestHandler<GetAllHolidaysQuery, ICollection<HolidayOverviewDto>>
    {
        private readonly IBaseRepository<Holiday> _holidayRepo;
        private readonly IMapper _mapper;

        public GetAllHolidaysQueryHandler(
            IBaseRepository<Holiday> holidayRepo, 
            IMapper mapper)
        {
            _holidayRepo = holidayRepo;
            _mapper = mapper;
        }

        public async Task<ICollection<HolidayOverviewDto>> Handle(GetAllHolidaysQuery request, CancellationToken cancellationToken)
        {
            var holidays = await _holidayRepo.GetAllByExpressionAsync(
                h => true);

            return _mapper.Map<ICollection<HolidayOverviewDto>>(holidays);
        }
    }
}
