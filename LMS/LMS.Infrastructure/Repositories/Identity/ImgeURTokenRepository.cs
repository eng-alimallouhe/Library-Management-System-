using LMS.Domain.Entities.HttpEntities;
using LMS.Infrastructure.DbContexts;

namespace LMS.Infrastructure.Repositories.Identity
{
    public class ImgurTokenRepository : BaseRepository<ImgurToken>
    {
        public ImgurTokenRepository(LMSDbContext context) : base(context) { }
    }
}
