using LMS.Application.DTOs.LibraryManagement.Authors;
using LMS.Application.DTOs.LibraryManagement.Categories;
using LMS.Application.DTOs.LibraryManagement.Genres;
using LMS.Application.DTOs.LibraryManagement.Publishers;

namespace LMS.Application.DTOs.LibraryManagement.Books
{
    public class BookToUpdate
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
        public ICollection<CategoryLookUpDto> Categories { get; set; } = [];
        public string ImgUrl { get; set; } = string.Empty;
        public ICollection<GenreLookupDto> Genres { get; set; } = [];
        public ICollection<PublisherLookupDto> Publishers { get; set; } = [];
        public AuthorLookupDto Author { get; set; } = new AuthorLookupDto();
    }
}
