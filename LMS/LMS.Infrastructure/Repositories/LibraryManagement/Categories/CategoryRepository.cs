using LMS.Application.Abstractions.Repositories;
using LMS.Domain.LibraryManagement.Models.Categories;
using LMS.Infrastructure.DbContexts;

namespace LMS.Infrastructure.Repositories.LibraryManagement.Categories
{
    public class CategoryRepository : SoftDeletableRepository<Category>
    {
        private readonly LMSDbContext _context;

        public CategoryRepository(LMSDbContext context) : base(context)
        {
            _context = context;
        }


        public override async Task SoftDeleteAsync(Guid id)
        {
            var cat = await _context.Categories.FindAsync(id);

            if (cat != null)
            {
                cat.IsActive = false;
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new EntryPointNotFoundException();
            }
        }
    }
}
