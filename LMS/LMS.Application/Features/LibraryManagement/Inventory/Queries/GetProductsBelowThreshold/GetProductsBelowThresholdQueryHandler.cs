using AutoMapper;
using LMS.Application.Abstractions.LibraryManagement;
using LMS.Application.DTOs.Admin.Dashboard;
using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.Stock;
using MediatR;

namespace LMS.Application.Features.LibraryManagement.Inventory.Queries.GetProductsBelowThreshold
{
    public class GetProductsBelowThresholdQueryHandler : IRequestHandler<GetProductsBelowThresholdQuery, PagedResult<StockSnapshotDto>>
    {
        private readonly IInventoryHelper _inventoryHelper;
        private readonly IMapper _mapper;

        public GetProductsBelowThresholdQueryHandler(
            IInventoryHelper inventoryHelper,
            IMapper mapper)
        {
            _inventoryHelper = inventoryHelper;
            _mapper = mapper;
        }

        public async Task<PagedResult<StockSnapshotDto>> Handle(GetProductsBelowThresholdQuery request, CancellationToken cancellationToken)
        {
            var result = await _inventoryHelper.GetLowStockThresholdAsync(request.Filter);

            var items = _mapper.Map<ICollection<StockSnapshotDto>>(result.items, 
                context =>
                {
                    context.Items["lang"] = request.Filter.Language;
                });

            return new PagedResult<StockSnapshotDto>
            {
                Items = items,
                TotalCount = result.count,
                CurrentPage = request.Filter.PageNumber!.Value,
                PageSize = request.Filter.PageSize!.Value
            };
        }
    }
}
