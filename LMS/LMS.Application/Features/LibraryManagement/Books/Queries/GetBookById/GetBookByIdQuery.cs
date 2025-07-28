using LMS.Application.DTOs.LibraryManagement.Books;
using LMS.Domain.Identity.Enums;
using MediatR;

namespace LMS.Application.Features.LibraryManagement.Books.Queries.GetBookById
{
    public record GetBookByIdQuery(
        Language Language,
        Guid BookId) : IRequest<BooksDetailsDto?>;
}
