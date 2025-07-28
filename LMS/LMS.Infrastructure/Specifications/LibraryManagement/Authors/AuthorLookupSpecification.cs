using System.Linq.Expressions;
using LMS.Domain.Abstractions.Specifications;
using LMS.Domain.LibraryManagement.Models.Authors;

namespace LMS.Infrastructure.Specifications.LibraryManagement.Authors
{
    public class AuthorLookupSpecification : ISpecification<Author>
    {
        public Expression<Func<Author, bool>>? Criteria => a => a.IsActive;

        public List<string> Includes => new List<string> { nameof(Author.Translations) };

        public bool IsTrackingEnabled => false;

        public Expression<Func<Author, object>>? OrderBy => null;

        public Expression<Func<Author, object>>? OrderByDescending => null;

        public int? Skip => null;

        public int? Take => null;
    }
}
