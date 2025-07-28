using LMS.Application.Abstractions.LibraryManagement;
using LMS.Application.DTOs.LibraryManagement.Products;
using MediatR;

namespace LMS.Application.Features.LibraryManagement.Products.Queries.GetProductById
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDetailsDto?>
    {
        private readonly IProductHelper _productHelper;

        public GetProductByIdQueryHandler(IProductHelper productHelper)
        {
            _productHelper = productHelper;
        }

        public async Task<ProductDetailsDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            return await _productHelper.GetProdyctByIdAsync(request.ProductId, request.language);
        }
    }
}
