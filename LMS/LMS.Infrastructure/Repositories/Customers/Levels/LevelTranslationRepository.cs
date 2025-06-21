using LMS.Domain.Customers.Models.Levels;
using LMS.Infrastructure.DbContexts;

namespace LMS.Infrastructure.Repositories.Customers.Levels
{
    public class LevelTranslationRepository : BaseRepository<LoyaltyLevelTransaltion>
    {
        public LevelTranslationRepository(LMSDbContext context) : base(context) { }
    }
}
