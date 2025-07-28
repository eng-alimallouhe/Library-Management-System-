using LMS.Application.DTOs.LibraryManagement.Categories;

namespace LMS.Application.DTOs.LibraryManagement.Products
{
    public class ProductToUpdateDto
    {
        public string ARProductName { get; set; }
        public string ARProductDescription { get; set; }
        public string ENProductName { get; set; }
        public string ENProductDescription { get; set; }
        public decimal ProductPrice { get; set; }
        public int ProductStock { get; set; }
        public ICollection<CategoryLookUpDto> Categories { get; set; } = [];
        public string ImgUrl { get; set; } = string.Empty;
    }
}
