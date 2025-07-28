using LMS.Application.Abstractions.LibraryManagement;
using LMS.Application.DTOs.LibraryManagement.Publishers;
using MediatR;

namespace LMS.Application.Features.LibraryManagement.Publishers.Queries.GetPublisherLookup
{
    public class GetPublisherLookupQueryHandler : IRequestHandler<GetPublisherLookupQuery, ICollection<PublisherLookupDto>>
    {
        private readonly IStockManagementHelper _stockManagementHelper;

        public GetPublisherLookupQueryHandler(IStockManagementHelper stockManagementHelper)
        {
            _stockManagementHelper = stockManagementHelper;
        }
        public async Task<ICollection<PublisherLookupDto>> Handle(GetPublisherLookupQuery request, CancellationToken cancellationToken)
        {
            return await _stockManagementHelper.GetAllPublishersAsync(request.Language);
        }
    }
}
