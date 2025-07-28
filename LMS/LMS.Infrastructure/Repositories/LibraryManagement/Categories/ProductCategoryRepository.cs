using LMS.Domain.LibraryManagement.Models.Relations;
using LMS.Infrastructure.DbContexts;

namespace LMS.Infrastructure.Repositories.LibraryManagement.Categories
{
    public class ProductCategoryRepository : BaseRepository<ProductCategory>
    {
        public ProductCategoryRepository(LMSDbContext context) : base(context) { }
    }
}
