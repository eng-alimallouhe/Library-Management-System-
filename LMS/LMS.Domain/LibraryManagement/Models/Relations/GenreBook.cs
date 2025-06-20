using LMS.Domain.LibraryManagement.Models.Genres;
using LMS.Domain.LibraryManagement.Models.Products;

namespace LMS.Domain.LibraryManagement.Models.Relations
{
    public class GenreBook
    {
        public Guid GenreBookId { get; set; }
        public Guid GenreId { get; set; }
        public Guid BookId { get; set; }


        public Book Book { get; set; }
        public Genre Genre { get; set; }

        public GenreBook()
        {
            GenreBookId = Guid.NewGuid();
            Book = null!;
            Genre = null!;
        }
    }
}
