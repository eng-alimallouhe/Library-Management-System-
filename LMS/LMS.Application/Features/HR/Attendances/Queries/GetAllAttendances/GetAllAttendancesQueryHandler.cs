using AutoMapper;
using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.HR;
using MediatR;

namespace LMS.Application.Features.HR.Attendances.Queries.GetAllAttendances
{
    public class GetAllAttendancesQueryHandler : IRequestHandler<GetAllAttendancesQuery, PagedResult<AttendanceOverviewDto>>
    {
        private readonly IEmployeeCompensationHelper _employeeCompensationHelper;
        private readonly IMapper _mapper;

        public GetAllAttendancesQueryHandler(
            IEmployeeCompensationHelper employeeCompensationHelper,
            IMapper mapper)
        {
            _employeeCompensationHelper = employeeCompensationHelper;
            _mapper = mapper;
        }

        public async Task<PagedResult<AttendanceOverviewDto>> Handle(GetAllAttendancesQuery request, CancellationToken cancellationToken)
        {
            (var items, var count) = await _employeeCompensationHelper.GetPagedAttendancesAsync(request.Filter);

            return new PagedResult<AttendanceOverviewDto>
            {
                Items = _mapper.Map<ICollection<AttendanceOverviewDto>>(items),
                TotalCount = count,
                CurrentPage = request.Filter.PageNumber!.Value,
                PageSize = request.Filter.PageSize!.Value
            };
        }
    }
}
