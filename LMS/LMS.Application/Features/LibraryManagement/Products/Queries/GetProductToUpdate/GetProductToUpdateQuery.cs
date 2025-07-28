using LMS.Application.DTOs.LibraryManagement.Products;
using LMS.Domain.Identity.Enums;
using MediatR;

namespace LMS.Application.Features.LibraryManagement.Products.Queries.GetProductToUpdate
{
    public record GetProductToUpdateQuery(
        Guid productId, 
        Language Language) : IRequest<ProductToUpdateDto?>;
}
