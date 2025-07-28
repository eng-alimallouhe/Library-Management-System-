using System.Linq.Expressions;
using LMS.Application.Filters.LibraryManagement;
using LMS.Domain.Abstractions.Specifications;
using LMS.Domain.Identity.Enums;
using LMS.Domain.LibraryManagement.Models.Products;

namespace LMS.Infrastructure.Specifications.LibraryManagement
{
    public class FilteredBooksSpecification : ISpecification<Book>
    {
        private readonly BooksFilter _filter;

        public Expression<Func<Book, bool>>? Criteria =>
        p =>
            (_filter.Name == null ||
                p.Translations.Any(t =>
                    t.ProductName.Contains(_filter.Name) &&
                    t.Language == (Language)_filter.Language)) &&

            (_filter.From == null || p.CreatedAt >= _filter.From) &&

            (_filter.To == null || p.CreatedAt <= _filter.To) &&

            (_filter.GeneresIds == null || !_filter.GeneresIds.Any() ||
            p.GenreBooks.Any(pc => _filter.GeneresIds.Contains(pc.GenreId))) &&

            (_filter.PublishersIds == null || !_filter.PublishersIds.Any() ||
            p.PublisherBooks.Any(pc => _filter.PublishersIds.Contains(pc.PublisherId))) &&

            (_filter.AuthorsIds == null || !_filter.AuthorsIds.Any() ||
            _filter.AuthorsIds.Contains(p.AuthorId));
        public List<string> Includes => new List<string>
        {
            "PublisherBooks.Publisher",
            "GenreBooks.Genre",
            "Author",
            "Translations"
        };

        public bool IsTrackingEnabled => false;

        public Expression<Func<Book, object>>? OrderBy => p => p.ISBN;

        public Expression<Func<Book, object>>? OrderByDescending => null;

        public int? Skip => _filter.PageNumber;

        public int? Take => _filter.PageSize;

        public FilteredBooksSpecification(BooksFilter filter)
        {
            _filter = filter;
        }
    }
}
