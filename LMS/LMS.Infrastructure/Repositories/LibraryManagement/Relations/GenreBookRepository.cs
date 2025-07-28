using LMS.Domain.LibraryManagement.Models.Relations;
using LMS.Infrastructure.DbContexts;

namespace LMS.Infrastructure.Repositories.LibraryManagement.Relations
{
    public class GenreBookRepository : BaseRepository<GenreBook>
    {
        public GenreBookRepository(LMSDbContext context) : base(context)
        {
        }
    }
}
