using LMS.Application.Filters.Base;

namespace LMS.Application.Filters.LibraryManagement
{
    public class BooksFilter : Filter
    {
        public List<Guid>? GeneresIds { get; set; } = [];
        public List<Guid>? PublishersIds { get; set; } = [];
        public List<Guid>? AuthorsIds { get; set; } = [];
    }
}
