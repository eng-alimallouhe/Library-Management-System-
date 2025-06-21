using LMS.Domain.Identity.Models;
using LMS.Infrastructure.DbContexts;

namespace LMS.Infrastructure.Repositories.Identity
{
    public class RefreshTokenRepository: BaseRepository<RefreshToken>
    {
        public RefreshTokenRepository(LMSDbContext context) : base(context) { }
    }
}
