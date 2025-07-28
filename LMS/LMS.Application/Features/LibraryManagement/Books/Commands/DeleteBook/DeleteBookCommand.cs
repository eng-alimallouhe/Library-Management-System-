using LMS.Common.Results;
using MediatR;

namespace LMS.Application.Features.LibraryManagement.Books.Commands.DeleteBook
{
    public class DeleteBookCommand : IRequest<Result>
    {
        public Guid BookId { get; set; }
    }
}
