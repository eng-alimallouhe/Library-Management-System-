using LMS.Domain.LibraryManagement.Models.Authors;
using LMS.Infrastructure.DbContexts;

namespace LMS.Infrastructure.Repositories.LibraryManagement.Authors
{
    public class AuthorTranslationRepository : BaseRepository<AuthorTranslation>
    {
        public AuthorTranslationRepository(LMSDbContext context) : base(context) {  }
    }
}