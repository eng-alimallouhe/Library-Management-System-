using LMS.Application.Abstractions.LibraryManagement;
using LMS.Application.DTOs.LibraryManagement.Books;
using MediatR;

namespace LMS.Application.Features.LibraryManagement.Books.Queries.GetBookToUpdate
{
    public class GetBookToUpdateQueryHandler : IRequestHandler<GetBookToUpdateQuery, BookToUpdate?>
    {

        private readonly IProductHelper _productHelper;

        public GetBookToUpdateQueryHandler(
            IProductHelper productHelper)
        {
            _productHelper = productHelper;
        }

        public async Task<BookToUpdate?> Handle(GetBookToUpdateQuery request, CancellationToken cancellationToken)
        {
            return await _productHelper.GetBookToUpdateAsync(request.BookId, request.language);
        }
    }
}
