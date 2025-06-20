using LMS.Domain.LibraryManagement.Models.Relations;

namespace LMS.Domain.LibraryManagement.Models.Genres
{
    public class Genre
    {
        // Primary key:
        public Guid GenreId { get; set; }


        //soft delete
        public bool IsActive { get; set; }


        //Timestamp:
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


        // Navigation property:
        public ICollection<GenreBook> GenreBooks { get; set; }
        public ICollection<GenreTranslation> Translations { get; set; }


        public Genre()
        {
            GenreId = Guid.NewGuid();
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            GenreBooks = [];
            Translations = [];
        }
    }
}
