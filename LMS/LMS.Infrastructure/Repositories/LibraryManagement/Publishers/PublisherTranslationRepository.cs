using LMS.Domain.LibraryManagement.Models.Publishers;
using LMS.Infrastructure.DbContexts;

namespace LMS.Infrastructure.Repositories.LibraryManagement.Publishers
{
    public class PublisherTranslationRepository : BaseRepository<PublisherTranslation>
    {
        public PublisherTranslationRepository(LMSDbContext context) : base(context) { }
    }
}
