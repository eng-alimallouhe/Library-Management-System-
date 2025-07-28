using AutoMapper;
using LMS.Application.Abstractions.HR;
using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.HR;
using MediatR;

namespace LMS.Application.Features.HR.Incentives.Queries.GetAllIncentives
{
    public class GetAllIncentivesQueryHandler : IRequestHandler<GetAllIncentivesQuery, PagedResult<IncentiveOverviewDto>>
    {
        private readonly IEmployeeCompensationHelper _compHelper;
        private readonly IMapper _mapper;

        public GetAllIncentivesQueryHandler(
            IEmployeeCompensationHelper compHelper,
            IMapper mapper)
        {
            _compHelper = compHelper;
            _mapper = mapper;
        }

        public async Task<PagedResult<IncentiveOverviewDto>> Handle(GetAllIncentivesQuery request, CancellationToken cancellationToken)
        {
            var (items, count) = await _compHelper.GetPagedIncentivesAsync(request.Filter);
            var dtos = _mapper.Map<ICollection<IncentiveOverviewDto>>(items);

            return new PagedResult<IncentiveOverviewDto>
            {
                Items = dtos,
                TotalCount = count,
                CurrentPage = request.Filter.PageNumber!.Value,
                PageSize = request.Filter.PageSize!.Value,
            };
        }
    }
}