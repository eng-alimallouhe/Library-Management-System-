using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.DTOs.LibraryManagement.Products
{
    public class ProductCreateRequestDto
    {
        [Required]
        public string ARProductName { get; set; }

        [Required]
        public string ARProductDescription { get; set; }

        [Required]
        public string ENProductName { get; set; }

        [Required]
        public string ENProductDescription { get; set; }

        [Required]
        public decimal ProductPrice { get; set; }

        [Required]
        public int ProductStock { get; set; }


        [Required]
        public ICollection<Guid> CategoriesIds { get; set; } = [];

        [Required]
        [FromForm]
        public IFormFile ImageFile { get; set; }
    }
}
