using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.Stock;
using LMS.Application.Filters.Inventory;
using MediatR;

namespace LMS.Application.Features.LibraryManagement.Inventory.Queries.GetProductsBelowThreshold
{
    public record GetProductsBelowThresholdQuery(
        ProductFilter Filter) : IRequest<PagedResult<StockSnapshotDto>>;
}
