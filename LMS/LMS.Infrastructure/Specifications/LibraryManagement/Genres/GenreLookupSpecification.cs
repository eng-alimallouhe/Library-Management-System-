using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LMS.Domain.Abstractions.Specifications;
using LMS.Domain.LibraryManagement.Models.Genres;

namespace LMS.Infrastructure.Specifications.LibraryManagement.Genres
{
    internal class GenreLookupSpecification : ISpecification<Genre>
    {
        public Expression<Func<Genre, bool>>? Criteria => g => g.IsActive;

        public List<string> Includes => new List<string>
        {
            nameof(Genre.Translations)
        };

        public bool IsTrackingEnabled => false;

        public Expression<Func<Genre, object>>? OrderBy => null;

        public Expression<Func<Genre, object>>? OrderByDescending => null;

        public int? Skip => null;

        public int? Take => null;
    }
}
