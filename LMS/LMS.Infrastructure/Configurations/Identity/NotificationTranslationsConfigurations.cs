using LMS.Domain.Identity.Models;
using LMS.Domain.Identity.Models.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Configurations.Users
{
    internal class NotificationTranslationsConfigurations : IEntityTypeConfiguration<NotificationTranslation>
    {
        public void Configure(EntityTypeBuilder<NotificationTranslation> builder)
        {
            builder.ToTable("NotificationsTranslation");

            builder.HasKey(n => n.TranslationId);

            builder.Property(n => n.NotificationId)
                    .IsRequired();


            builder.HasOne<Notification>()
                    .WithMany(u => u.Translations)
                    .HasForeignKey(n => n.NotificationId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
        }
    }
}