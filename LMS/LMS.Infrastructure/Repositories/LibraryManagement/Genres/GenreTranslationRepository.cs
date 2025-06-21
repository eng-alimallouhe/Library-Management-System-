using LMS.Domain.LibraryManagement.Models.Genres;
using LMS.Infrastructure.DbContexts;

namespace LMS.Infrastructure.Repositories.LibraryManagement.Genres
{
    public class GenreTranslationRepository : BaseRepository<GenreTranslation>
    {
        public GenreTranslationRepository(LMSDbContext context) : base(context) { }
    }
}
