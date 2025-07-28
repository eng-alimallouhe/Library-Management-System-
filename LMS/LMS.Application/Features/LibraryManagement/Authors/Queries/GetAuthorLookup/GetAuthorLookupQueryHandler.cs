using LMS.Application.Abstractions.LibraryManagement;
using LMS.Application.DTOs.LibraryManagement.Authors;
using MediatR;

namespace LMS.Application.Features.LibraryManagement.Authors.Queries.GetAuthorLookup
{
    public class GetAuthorLookupQueryHandler : IRequestHandler<GetAuthorLookupQuery, ICollection<AuthorLookupDto>>
    {
        private readonly IStockManagementHelper _stockManagementHelper;

        public GetAuthorLookupQueryHandler(IStockManagementHelper stockManagementHelper)
        {
            _stockManagementHelper = stockManagementHelper;
        }

        public async Task<ICollection<AuthorLookupDto>> Handle(GetAuthorLookupQuery request, CancellationToken cancellationToken)
        {
            return await _stockManagementHelper.GetAllAuthorsAsync(request.Language);
        }
    }
}
