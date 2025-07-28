using AutoMapper;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.DTOs.HR;
using LMS.Domain.HR.Models;
using MediatR;

namespace LMS.Application.Features.HR.Holidays.Queries.GetHolidayById
{
    public class GetHolidayByIdQueryHandler : IRequestHandler<GetHolidayByIdQuery, HolidayDetailsDto?>
    {
        private readonly IBaseRepository<Holiday> _holidayRepo;
        private readonly IMapper _mapper;

        public GetHolidayByIdQueryHandler(
            IBaseRepository<Holiday> holidayRepo,
            IMapper mapper)
        {
            _holidayRepo = holidayRepo;
            _mapper = mapper;
        }

        public async Task<HolidayDetailsDto?> Handle(GetHolidayByIdQuery request, CancellationToken cancellationToken)
        {
            var holiday = await _holidayRepo.GetByIdAsync(request.HolidayId);

            if (holiday is null)
                return null;

            return _mapper.Map<HolidayDetailsDto>(holiday);
        }
    }
}
