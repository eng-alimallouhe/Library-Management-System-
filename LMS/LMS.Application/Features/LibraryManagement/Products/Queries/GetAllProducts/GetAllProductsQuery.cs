using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.LibraryManagement.Products;
using LMS.Application.Filters.Inventory;
using MediatR;

namespace LMS.Application.Features.LibraryManagement.Products.Queries.GetAllProducts
{
    public record GetAllProductsQuery(
        ProductFilter Filter) : IRequest<PagedResult<ProductOverviewDto>>;
}
