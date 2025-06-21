using LMS.Domain.LibraryManagement.Models.Products;
using LMS.Infrastructure.DbContexts;

namespace LMS.Infrastructure.Repositories.LibraryManagement.Products
{
    public class ProductTranslationRepository : BaseRepository<ProductTranslation>
    {
        public ProductTranslationRepository(LMSDbContext context) : base(context) { }
    }
}
