using LMS.Domain.LibraryManagement.Models.Relations;
using LMS.Infrastructure.DbContexts;

namespace LMS.Infrastructure.Repositories.LibraryManagement.Relations
{
    public class PublisherBookRepository : BaseRepository<PublisherBook>
    {
        public PublisherBookRepository(LMSDbContext context) : base(context)
        {
        }
    }
}
