namespace LMS.Domain.Identity.Models.Notifications
{
    public class Notification
    {
        //primary key: 
        public Guid NotificationId { get; set; }

        
        // Foreign Key: UserId ==> one(user) to many(notifications) relationship
        public Guid UserId { get; set; }
       
        public DateTime CreatedAt { get; set; } 
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public string? RedirectUrl { get; set; }


        public ICollection<NotificationTranslation> Translations { get; set; }



        public Notification()
        {
            NotificationId = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            IsRead = false;
            Translations = [];
        }

    }
}
