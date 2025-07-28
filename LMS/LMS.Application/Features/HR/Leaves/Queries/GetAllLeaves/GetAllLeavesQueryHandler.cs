using AutoMapper;
using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.HR;
using MediatR;

namespace LMS.Application.Features.HR.Leaves.Queries.GetAllLeaves
{
    public class GetAllLeavesQueryHandler : IRequestHandler<GetAllLeavesQuery, PagedResult<LeaveOverviewDto>>
    {
        private readonly IEmployeeCompensationHelper _employeeCompensationHelper;
        private readonly IMapper _mapper;

        public GetAllLeavesQueryHandler(IEmployeeCompensationHelper employeeCompensationHelper, IMapper mapper)
        {
            _employeeCompensationHelper = employeeCompensationHelper;
            _mapper = mapper;
        }

        public async Task<PagedResult<LeaveOverviewDto>> Handle(GetAllLeavesQuery request, CancellationToken cancellationToken)
        {
            var (leaves, count) = await _employeeCompensationHelper.GetFilteredLeavesAsync(request.Filter);

            var dtos = _mapper.Map<ICollection<LeaveOverviewDto>>(leaves);

            return new PagedResult<LeaveOverviewDto>
            {
                Items = dtos,
                TotalCount = count,
                CurrentPage = request.Filter.PageNumber!.Value,
                PageSize = request.Filter.PageSize!.Value,
            };
        }
    }

}
