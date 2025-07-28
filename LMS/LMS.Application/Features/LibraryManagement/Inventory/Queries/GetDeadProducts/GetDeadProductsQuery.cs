using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.Stock;
using LMS.Application.Filters.Inventory;
using MediatR;

namespace LMS.Application.Features.LibraryManagement.Inventory.Queries.GetDeadProducts
{
    public record GetDeadProductsQuery(
        DeadStockFilter Filter) : IRequest<PagedResult<DeadStockDto>>;
}