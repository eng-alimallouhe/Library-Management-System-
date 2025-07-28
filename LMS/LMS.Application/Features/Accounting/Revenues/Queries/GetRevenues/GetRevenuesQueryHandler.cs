using AutoMapper;
using LMS.Application.Abstractions.Accounting;
using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.Financial;
using MediatR;

namespace LMS.Application.Features.Accounting.Revenues.Queries.GetRevenues
{
    public class GetRevenuesQueryHandler : IRequestHandler<GetRevenuesQuery, PagedResult<RevenueDetailsDto>>
    {
        private readonly IFinancialHelper _financialHelper;
        private readonly IMapper _mapper;

        public GetRevenuesQueryHandler(
            IFinancialHelper financialHelper,
            IMapper mapper)
        {
            _financialHelper = financialHelper;
            _mapper = mapper;
        }

        public async Task<PagedResult<RevenueDetailsDto>> Handle(GetRevenuesQuery request, CancellationToken cancellationToken)
        {
            var response = await _financialHelper.GetFilteredRevenueAsync(request.Filter);

            var items = _mapper.Map<ICollection<RevenueDetailsDto>>(response);

            return new PagedResult<RevenueDetailsDto>
            {
                Items = items,
                TotalCount = response.count,
                CurrentPage = request.Filter.PageNumber!.Value,
                PageSize = request.Filter.PageSize!.Value
            };
        }
    }
}
