using LMS.Domain.Identity.Enums;

namespace LMS.Domain.LibraryManagement.Models.Publishers
{
    public class PublisherTranslation
    {
        // Primary key:
        public Guid TranslationId { get; set; }


        //Foreign Key: PublisherId ==> one(publisher)-to-many(translation) relationship
        public Guid PublisherId { get; set; }


        public Language Language { get; set; }
        public required string PublisherName { get; set; }
        public required string PublisherDescription { get; set; }


        //Navigation Property:
        public Publisher Publisher { get; set; }

        public PublisherTranslation()
        {
            TranslationId = Guid.NewGuid();
            Publisher = null!;
        }
    }

}
