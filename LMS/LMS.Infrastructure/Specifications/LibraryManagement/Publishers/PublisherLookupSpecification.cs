using System.Linq.Expressions;
using LMS.Domain.Abstractions.Specifications;
using LMS.Domain.LibraryManagement.Models.Publishers;

namespace LMS.Infrastructure.Specifications.LibraryManagement.Publishers
{
    public class PublisherLookupSpecification : ISpecification<Publisher>
    {
        public Expression<Func<Publisher, bool>>? Criteria => p => p.IsActive;

        public List<string> Includes => new List<string>
        {
            nameof(Publisher.Translations)
        };

        public bool IsTrackingEnabled => false;

        public Expression<Func<Publisher, object>>? OrderBy => null;

        public Expression<Func<Publisher, object>>? OrderByDescending => null;

        public int? Skip => null;

        public int? Take => null;
    }
}
