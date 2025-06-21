using LMS.Domain.Identity.Models;
using LMS.Infrastructure.DbContexts;

namespace LMS.Infrastructure.Repositories.Identity
{
    public class OtpCodeRepository : BaseRepository<OtpCode>
    {
        public OtpCodeRepository(LMSDbContext context) : base(context) { }
    }
}
