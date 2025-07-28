using LMS.Application.Features.LibraryManagement.Books.Commands.UpdateBook;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.DTOs.LibraryManagement.Books
{
    public class BookUpdateRequestDto
    {
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

        [FromForm]
        public IFormFile? ImageFile { get; set; }
        public ICollection<Guid> Genres { get; set; } = [];
        public ICollection<Guid> Publishers { get; set; } = [];
        public Guid Author { get; set; }
    }
}
