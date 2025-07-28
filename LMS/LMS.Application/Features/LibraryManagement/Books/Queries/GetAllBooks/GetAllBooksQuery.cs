using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.LibraryManagement.Products;
using LMS.Application.Filters.LibraryManagement;
using MediatR;

namespace LMS.Application.Features.LibraryManagement.Books.Queries.GetAllBooks
{
    public record GetAllBooksQuery(BooksFilter Filter) : IRequest<PagedResult<ProductOverviewDto>>;
}
