using LMS.Domain.LibraryManagement.Models.Products;

namespace LMS.Application.DTOs.LibraryManagement.Products
{
    public class ProductOverviewDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal ProductPrice { get; set; }
        public int ProductStock { get; set; }
        public string ImgUrl { get; set; } = string.Empty;
        public decimal DiscountPercentage { get; set; }
        public string ProductDescription { get; set; }
    }
}
