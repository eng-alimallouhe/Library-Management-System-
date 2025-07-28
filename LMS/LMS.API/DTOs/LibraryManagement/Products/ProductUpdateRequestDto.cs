using Microsoft.AspNetCore.Mvc;

namespace LMS.API.DTOs.LibraryManagement.Products
{
    public class ProductUpdateRequestDto
    {
        public string ARProductName { get; set; }

        public string ARProductDescription { get; set; }

        public string ENProductName { get; set; }

        public string ENProductDescription { get; set; }

        public decimal ProductPrice { get; set; }

        public int ProductStock { get; set; }


        public ICollection<Guid> CategoriesIds { get; set; } = [];

        [FromForm]
        public IFormFile? ImageFile { get; set; }
    }
}
