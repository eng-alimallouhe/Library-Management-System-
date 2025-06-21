using LMS.Domain.Identity.Models;
using LMS.Infrastructure.DbContexts;

namespace LMS.Infrastructure.Repositories.Identity
{

    public class NotificationRepository : BaseRepository<Notification>
    {
        public NotificationRepository(LMSDbContext context) : base(context) { }
    }
}
