using LMS.Application.DTOs.LibraryManagement.Products;
using MediatR;

namespace LMS.Application.Features.LibraryManagement.Products.Queries.GetProductById
{
    public record GetProductByIdQuery(
        Guid ProductId,
        int language) : IRequest<ProductDetailsDto?>;
}
