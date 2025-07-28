using AutoMapper;
using LMS.Application.Abstractions.Accounting;
using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.Stock;
using MediatR;

namespace LMS.Application.Features.LibraryManagement.Inventory.Queries.GetDeadProducts
{
    public class GetDeadProductsQueryHandler : IRequestHandler<GetDeadProductsQuery, PagedResult<DeadStockDto>>
    {
        private readonly IFinancialHelper _financialHelper;
        private readonly IMapper _mapper;

        public GetDeadProductsQueryHandler(
            IFinancialHelper financialHelper,
            IMapper mapper)
        {
            _financialHelper = financialHelper;
            _mapper = mapper;
        }

        public async Task<PagedResult<DeadStockDto>> Handle(GetDeadProductsQuery request, CancellationToken cancellationToken)
        {
            var response = await _financialHelper.GetDeadStockAsync(request.Filter);

            var items = _mapper.Map<ICollection<DeadStockDto>>(response);

            return new PagedResult<DeadStockDto>
            {
                Items = items,
                TotalCount = response.count,
                CurrentPage = request.Filter.PageNumber ?? 1,
                PageSize = request.Filter.PageSize ?? 10
            };
        }
    }
}
