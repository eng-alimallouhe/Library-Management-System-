using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.Stock;
using LMS.Application.Filters.Inventory;
using MediatR;

namespace LMS.Application.Features.LibraryManagement.Movements.Queries.GetStockChangeLogs
{
    public record GetStockChangeLogsQuery(
        InventoryLogsFilter Filter) : IRequest<PagedResult<InventoryLogOverviewDto>>;
}
