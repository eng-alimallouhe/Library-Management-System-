using AutoMapper;
using LMS.Application.Abstractions.LibraryManagement;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.DTOs.LibraryManagement.Categories;
using LMS.Application.DTOs.LibraryManagement.Products;
using LMS.Domain.LibraryManagement.Models.Products;
using MediatR;

namespace LMS.Application.Features.LibraryManagement.Products.Queries.GetProductToUpdate
{
    public class GetProductToUpdateQueryHandler : IRequestHandler<GetProductToUpdateQuery, ProductToUpdateDto?>
    {
        private readonly ISoftDeletableRepository<Product> _productRepo;
        private readonly IProductHelper productHelper;

        public GetProductToUpdateQueryHandler(
            ISoftDeletableRepository<Product> productRepo,
            IProductHelper productHelper)
        {
            _productRepo = productRepo;
            this.productHelper = productHelper;
        }

        public async Task<ProductToUpdateDto?> Handle(GetProductToUpdateQuery request, CancellationToken cancellationToken)
        {
            var result = await productHelper.GetProductAsync(request.productId);

            if (result ==  null)
            {
                return null;
            }

            var product = new ProductToUpdateDto
            {
                ARProductName = result.Translations.FirstOrDefault(t => t.Language == Domain.Identity.Enums.Language.AR)?.ProductName ?? "N/A",
                ARProductDescription = result.Translations.FirstOrDefault(t => t.Language == Domain.Identity.Enums.Language.AR)?.ProductDescription ?? "N/A",
                ENProductName = result.Translations.FirstOrDefault(t => t.Language == Domain.Identity.Enums.Language.EN)?.ProductName ?? "N/A",
                ENProductDescription = result.Translations.FirstOrDefault(t => t.Language == Domain.Identity.Enums.Language.EN)?.ProductDescription ?? "N/A",
                ProductPrice = result.ProductPrice,
                ProductStock = result.ProductStock,
                ImgUrl = result.ImgUrl,
                Categories = result.ProductCategoriys.Select(t => new CategoryLookUpDto
                {
                    CategoryId = t.CategoryId,
                    CategoryName = t.Category.Translations.FirstOrDefault(t => t.Language == request.Language)?.CategoryName ?? "N/A"
                }).ToList()
            };

            return product;
        }
    }
}
