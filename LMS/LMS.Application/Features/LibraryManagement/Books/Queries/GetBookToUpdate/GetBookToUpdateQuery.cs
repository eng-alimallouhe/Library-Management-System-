using LMS.Application.DTOs.LibraryManagement.Books;
using LMS.Domain.Identity.Enums;
using MediatR;

namespace LMS.Application.Features.LibraryManagement.Books.Queries.GetBookToUpdate
{
    public record GetBookToUpdateQuery(
        Guid BookId,
        Language language) : IRequest<BookToUpdate?>;
}
