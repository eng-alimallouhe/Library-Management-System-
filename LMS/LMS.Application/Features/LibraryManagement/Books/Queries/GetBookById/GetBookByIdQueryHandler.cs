using LMS.Application.Abstractions.LibraryManagement;
using LMS.Application.DTOs.LibraryManagement.Books;
using MediatR;

namespace LMS.Application.Features.LibraryManagement.Books.Queries.GetBookById
{
    public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, BooksDetailsDto?>
    {
        private readonly IProductHelper _productHelper;

        public GetBookByIdQueryHandler(
            IProductHelper productHelper)
        {
            _productHelper = productHelper;
        }

        public async Task<BooksDetailsDto?> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
        {
            return await _productHelper.GetBookAsync(request.BookId, request.Language);
        }
    }
}
