using LMS.Domain.Identity.Enums;

namespace LMS.Application.DTOs.LibraryManagement.Products
{
    public class ProductTranslationsOverview
    {
        public Language Language { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
    }
}
