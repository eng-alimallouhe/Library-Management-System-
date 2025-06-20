using LMS.Domain.LibraryManagement.Models.Relations;

namespace LMS.Domain.LibraryManagement.Models.Publishers
{
    public class Publisher
    {
        // Primary key:
        public Guid PublisherId { get; set; }


        //soft delete
        public bool IsActive { get; set; }


        //Timestamp:
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


        // Navigation property:
        public ICollection<PublisherBook> PublisherBooks { get; set; }
        public ICollection<PublisherTranslation> Translations { get; set; }


        public Publisher()
        {
            PublisherId = Guid.NewGuid();
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            PublisherBooks = [];
            Translations = [];
        }
    }

}
