using AutoMapper;
using LMS.Application.Abstractions.LibraryManagement;
using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.Stock;
using MediatR;

namespace LMS.Application.Features.LibraryManagement.Inventory.Queries.GetInventoryAudit
{
    public class GetInventoryAuditQueryHandler : IRequestHandler<GetInventoryAuditQuery, PagedResult<StockSnapshotDto>>
    {

        private readonly IInventoryHelper _inventoryHelper;
        private readonly IMapper _mapper;

        public GetInventoryAuditQueryHandler(
            IInventoryHelper inventoryHelper,
            IMapper mapper)
        {
            _inventoryHelper = inventoryHelper;
            _mapper = mapper;
        }

        public async Task<PagedResult<StockSnapshotDto>> Handle(GetInventoryAuditQuery request, CancellationToken cancellationToken)
        {
            var response = await _inventoryHelper.GetInventorySnapshot(request.Filter);

            var items = _mapper.Map<ICollection<StockSnapshotDto>>(response.items);

            return new PagedResult<StockSnapshotDto>
            {
                Items = items,
                TotalCount = response.count,
                CurrentPage = request.Filter.PageNumber!.Value,
                PageSize = request.Filter.PageSize!.Value
            };
        }
    }
}
