using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LMS.Domain.Identity.Enums;

namespace LMS.Domain.Identity.Models.Notifications
{
    public class NotificationTranslation
    {
        public Guid TranslationId { get; set; }

        public Guid NotificationId { get; set; }
        public Language Language { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        public NotificationTranslation()
        {
            TranslationId = Guid.NewGuid();
        }
    }
}
