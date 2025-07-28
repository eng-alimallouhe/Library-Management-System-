using LMS.Application.Abstractions.LibraryManagement;
using LMS.Application.DTOs.LibraryManagement.Categories;
using MediatR;

namespace LMS.Application.Features.LibraryManagement.Categories.Queries.GetCategoriesLookup
{
    public class GetCategoriesLookupQueryHandler : IRequestHandler<GetCategoriesLookupQuery, ICollection<CategoryLookUpDto>>
    {
        private readonly IStockManagementHelper _stockManagementHelper;

        public GetCategoriesLookupQueryHandler(IStockManagementHelper stockManagementHelper)
        {
            _stockManagementHelper = stockManagementHelper;
        }

        public async Task<ICollection<CategoryLookUpDto>> Handle(GetCategoriesLookupQuery request, CancellationToken cancellationToken)
        {
            return await _stockManagementHelper.GetAllCategoriesAsync(request.Language);
        }
    }
}
