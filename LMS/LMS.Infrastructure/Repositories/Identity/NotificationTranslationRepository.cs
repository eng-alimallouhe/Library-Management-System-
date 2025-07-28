using LMS.Domain.Identity.Models.Notifications;
using LMS.Infrastructure.DbContexts;

namespace LMS.Infrastructure.Repositories.Identity
{

    public class NotificationTranslationRepository : BaseRepository<NotificationTranslation>
    {
        public NotificationTranslationRepository(LMSDbContext context) : base(context) { }
    }
}
