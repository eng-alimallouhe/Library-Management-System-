using System.Linq.Expressions;
using LMS.Domain.Abstractions.Specifications;
using LMS.Domain.LibraryManagement.Models.Categories;

namespace LMS.Infrastructure.Specifications.LibraryManagement.Categories
{
    public class CategoryLookupSpecification : ISpecification<Category>
    {
        public Expression<Func<Category, bool>>? Criteria => c => c.IsActive;

        public List<string> Includes => new List<string>
            {
                nameof(Category.Translations)
            };

        public bool IsTrackingEnabled => false;

        public Expression<Func<Category, object>>? OrderBy => c => c.Translations.FirstOrDefault(r => r.Language == Domain.Identity.Enums.Language.EN)!.CategoryName;

        public Expression<Func<Category, object>>? OrderByDescending => null;

        public int? Skip => null;

        public int? Take => null;
    }
}
