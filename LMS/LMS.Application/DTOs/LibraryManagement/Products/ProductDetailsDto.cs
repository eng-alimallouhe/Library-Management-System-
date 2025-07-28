using LMS.Application.DTOs.LibraryManagement.Categories;
using LMS.Application.DTOs.Stock;

namespace LMS.Application.DTOs.LibraryManagement.Products
{
    public class ProductDetailsDto
    {
        public Guid ProductId { get; set; }

        public decimal ProductPrice { get; set; }
        public int ProductStock { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
        public string ImgUrl { get; set; } = string.Empty;
        public decimal DiscountPercentage { get; set; }
        public ICollection<CategoryLookUpDto> Categories { get; set; } = [];
        public ICollection<InventoryLogOverviewDto> Logs { get; set; } = [];
    }
}
