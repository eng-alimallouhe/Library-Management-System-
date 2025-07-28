using AutoMapper;
using LMS.Application.Abstractions.LibraryManagement;
using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.LibraryManagement.Products;
using MediatR;

namespace LMS.Application.Features.LibraryManagement.Products.Queries.GetAllProducts
{
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, PagedResult<ProductOverviewDto>>
    {
        private readonly IMapper _mapper;
        private readonly IProductHelper _productHelper;

        public GetAllProductsQueryHandler(
            IMapper mapper,
            IProductHelper productHelper)
        {
            _mapper = mapper;
            _productHelper = productHelper;
        }

        public async Task<PagedResult<ProductOverviewDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var (items, count) = await _productHelper.GetAllProducts(request.Filter);

            return new PagedResult<ProductOverviewDto>()
            {
                Items = _mapper.Map<ICollection<ProductOverviewDto>>(items, context =>
                {
                    context.Items["lang"] = request.Filter.Language;
                }),

                TotalCount = count,
                CurrentPage = request.Filter.PageNumber ?? 1,
                PageSize = request.Filter.PageSize ?? 10
            };
        }
    }
}
