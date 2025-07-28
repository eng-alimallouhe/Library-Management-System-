using AutoMapper;
using LMS.Application.Abstractions.LibraryManagement;
using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.Stock;
using MediatR;

namespace LMS.Application.Features.LibraryManagement.Movements.Queries.GetStockChangeLogs
{
    public class GetInventoryLogsQueryHandler : IRequestHandler<GetStockChangeLogsQuery, PagedResult<InventoryLogOverviewDto>>
    {
        private readonly IInventoryHelper _inventoryHelper;
        private readonly IMapper _mapper;

        public GetInventoryLogsQueryHandler(
            IInventoryHelper inventoryHelper,
            IMapper mapper)
        {
            _inventoryHelper = inventoryHelper;
            _mapper = mapper;
        }

        public async Task<PagedResult<InventoryLogOverviewDto>> Handle(GetStockChangeLogsQuery request, CancellationToken cancellationToken)
        {
            if (request.Filter.PageSize is null ||
                request.Filter.PageNumber is null)
            {
                return new PagedResult<InventoryLogOverviewDto>();
            }

            
            var response = await _inventoryHelper.GetInventoryLogsAsync(request.Filter);

            var items = _mapper.Map<ICollection<InventoryLogOverviewDto>>(response.items);

            return new PagedResult<InventoryLogOverviewDto>
            {
                Items = items,
                TotalCount = response.count,
                CurrentPage = request.Filter.PageNumber!.Value,
                PageSize = request.Filter.PageSize!.Value
            };
        }
    }
}
