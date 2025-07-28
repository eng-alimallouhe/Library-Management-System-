using LMS.Application.DTOs.LibraryManagement.Authors;
using LMS.Application.DTOs.LibraryManagement.Categories;
using LMS.Application.DTOs.LibraryManagement.Genres;
using LMS.Application.DTOs.LibraryManagement.Publishers;
using LMS.Application.DTOs.Stock;

namespace LMS.Application.DTOs.LibraryManagement.Books
{
    public class BooksDetailsDto
    {
        public Guid BookId { get; set; }
        public int Pages { get; set; }
        public int PublishedYear { get; set; }
        public decimal ProductPrice { get; set; }
        public int ProductStock { get; set; }
        public string ISBN { get; set; } = string.Empty;
        public string BookName { get; set; } = string.Empty;
        public string BookDescription { get; set; } = string.Empty;
        public string ImgUrl { get; set; } = string.Empty;
        public decimal DiscountPercentage { get; set; }
        public ICollection<CategoryLookUpDto> Categories { get; set; } = [];
        public ICollection<GenreLookupDto> Genres { get; set; } = [];
        public ICollection<PublisherLookupDto> Publishers { get; set; } = [];
        public ICollection<InventoryLogOverviewDto> Logs { get; set; } = [];
        public AuthorLookupDto Author { get; set; } = new AuthorLookupDto();
    }
}
