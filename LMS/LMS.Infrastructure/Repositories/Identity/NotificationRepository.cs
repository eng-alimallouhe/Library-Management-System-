﻿using LMS.Domain.Identity.Models.Notifications;
using LMS.Infrastructure.DbContexts;

namespace LMS.Infrastructure.Repositories.Identity
{

    public class NotificationRepository : BaseRepository<Notification>
    {
        public NotificationRepository(LMSDbContext context) : base(context) { }
    }
}
