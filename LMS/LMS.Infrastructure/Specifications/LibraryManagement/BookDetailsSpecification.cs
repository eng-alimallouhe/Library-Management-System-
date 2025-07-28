using System.Linq.Expressions;
using LMS.Domain.Abstractions.Specifications;
using LMS.Domain.LibraryManagement.Models.Products;

namespace LMS.Infrastructure.Specifications.LibraryManagement
{
    public class BookDetailsSpecification : ISpecification<Book>
    {
        private readonly Guid _id;

        public Expression<Func<Book, bool>>? Criteria => b => b.ProductId == _id;

        public List<string> Includes => 
            [
                "PublisherBooks.Publisher.Translations",
                "GenreBooks.Genre.Translations",
                "Author.Translations",
                "Translations"
            ];

        public bool IsTrackingEnabled => false;

        public Expression<Func<Book, object>>? OrderBy => null;

        public Expression<Func<Book, object>>? OrderByDescending => null;

        public int? Skip => null;

        public int? Take => null;

        public BookDetailsSpecification(Guid id)
        {
            _id = id;
        }
    }
}
