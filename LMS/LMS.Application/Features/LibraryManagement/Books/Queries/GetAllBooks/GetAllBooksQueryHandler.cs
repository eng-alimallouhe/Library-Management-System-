using AutoMapper;
using LMS.Application.Abstractions.LibraryManagement;
using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.LibraryManagement.Products;
using LMS.Domain.Identity.Enums;
using MediatR;

namespace LMS.Application.Features.LibraryManagement.Books.Queries.GetAllBooks
{
    public class GetAllBooksQueryHandler : IRequestHandler<GetAllBooksQuery, PagedResult<ProductOverviewDto>>
    {
        private readonly IProductHelper _productHelper;
        private readonly IMapper _mapper;

        public GetAllBooksQueryHandler(
            IProductHelper productHelper,
            IMapper mapper)
        {
            _productHelper = productHelper;
            _mapper = mapper;
        }

        public async Task<PagedResult<ProductOverviewDto>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
        {
            var (items, count) = await _productHelper.GetAllBooksAsync(request.Filter);

            return new PagedResult<ProductOverviewDto>
            {
                Items = items.Select(b => new ProductOverviewDto 
                { 
                    ProductId = b.ProductId,
                    ProductName = b.Translations.FirstOrDefault(b => b.Language == (Language) request.Filter.Language)?.ProductName ?? "N/A",
                    ProductPrice = b.ProductPrice,
                    ProductStock = b.ProductStock,
                    ImgUrl = b.ImgUrl
                }).ToList(),
                TotalCount = count,
                CurrentPage = request.Filter.PageNumber ?? 1,
                PageSize = request.Filter.PageSize ?? 10
            };
        }
    }
}
