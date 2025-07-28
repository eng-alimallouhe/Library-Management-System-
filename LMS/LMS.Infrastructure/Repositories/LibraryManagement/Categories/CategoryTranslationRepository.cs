using LMS.Application.Abstractions.Repositories;
using LMS.Domain.LibraryManagement.Models.Categories;
using LMS.Infrastructure.DbContexts;

namespace LMS.Infrastructure.Repositories.LibraryManagement.Categories
{
    public class CategoryTranslationRepository : BaseRepository<CategoryTranslation>
    {
        public CategoryTranslationRepository(LMSDbContext context) : base(context) { }
    }
}
