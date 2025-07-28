using LMS.Common.Results;
using MediatR;

namespace LMS.Application.Features.LibraryManagement.Books.Commands.UpdateBook
{
    public class UpdateBookCommand : IRequest<Result>
    {
        public Guid BookId { get; set; } = Guid.NewGuid();
        public string ARProductName { get; set; }
        public string ARProductDescription { get; set; }
        public string ENProductName { get; set; }
        public string ENProductDescription { get; set; }
        public decimal ProductPrice { get; set; }
        public int ProductStock { get; set; }
        public string ISBN { get; set; }
        public int Pages { get; set; }
        public int PublishedYear { get; set; }
        public ICollection<Guid> Categories { get; set; } = [];
        public byte[]? ImageByte { get; set; } = [];
        public ICollection<Guid> Genres { get; set; } = [];
        public ICollection<Guid> Publishers { get; set; } = [];
        public Guid Author { get; set; }
    }
}