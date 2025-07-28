using LMS.Domain.LibraryManagement.Models.Categories;

namespace LMS.Application.DTOs.LibraryManagement.Categories
{
    public class CategoryLookUpDto
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
