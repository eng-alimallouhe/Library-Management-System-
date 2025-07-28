using LMS.Application.Abstractions.LibraryManagement;
using LMS.Application.DTOs.LibraryManagement.Genres;
using MediatR;

namespace LMS.Application.Features.LibraryManagement.Genres.Queries.GenreLookup
{
    public class GenreLookupQueryHandler : IRequestHandler<GenreLookupQuery, ICollection<GenreLookupDto>>
    {
        private readonly IStockManagementHelper _stockManagementHelper;

        public GenreLookupQueryHandler(IStockManagementHelper stockManagementHelper)
        {
            _stockManagementHelper = stockManagementHelper;
        }


        public async Task<ICollection<GenreLookupDto>> Handle(GenreLookupQuery request, CancellationToken cancellationToken)
        {
            return await _stockManagementHelper.GetAllGenresAsync(request.Language);
        }
    }
}
