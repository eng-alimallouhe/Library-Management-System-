using LMS.Domain.LibraryManagement.Models.Products;
using LMS.Domain.LibraryManagement.Models.Publishers;

namespace LMS.Domain.LibraryManagement.Models.Relations
{
    public class PublisherBook
    {
        public Guid PublisherBookId { get; set; }
        public Guid PublisherId { get; set; }
        public Guid BookId { get; set; }


        public Book Book { get; set; }
        public Publisher Publisher { get; set; }


        public PublisherBook()
        {
            PublisherBookId = Guid.NewGuid();
            Book = null!;
            Publisher = null!;
        }
    }
}