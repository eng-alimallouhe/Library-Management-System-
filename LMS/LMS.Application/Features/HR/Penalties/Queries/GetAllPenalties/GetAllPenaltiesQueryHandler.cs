using AutoMapper;
using LMS.Application.Abstractions.HR;
using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.HR;
using MediatR;

namespace LMS.Application.Features.HR.Penalties.Queries.GetAllPenalties
{
    public class GetAllPenaltiesQueryHandler : IRequestHandler<GetAllPenaltiesQuery, PagedResult<PenaltyOverviewDto>>
    {
        private readonly IEmployeeCompensationHelper _employeeCompensationHelper;
        private readonly IMapper _mapper;

        public GetAllPenaltiesQueryHandler(
            IEmployeeCompensationHelper employeeCompensationHelper,
            IMapper mapper)
        {
            _employeeCompensationHelper = employeeCompensationHelper;
            _mapper = mapper;
        }

        public async Task<PagedResult<PenaltyOverviewDto>> Handle(GetAllPenaltiesQuery request, CancellationToken cancellationToken)
        {
            var response = await _employeeCompensationHelper.GetPagedPenaltiesAsync(request.Filter);

            var items = _mapper.Map<ICollection<PenaltyOverviewDto>>(response.items);

            return new PagedResult<PenaltyOverviewDto>
            {
                Items = items,
                TotalCount = response.count,
                CurrentPage = request.Filter.PageNumber!.Value,
                PageSize = request.Filter.PageSize!.Value,
            };
        }
    }
}
