using LMS.Domain.LibraryManagement.Models.Categories;
using LMS.Domain.LibraryManagement.Models.Products;

namespace LMS.Domain.LibraryManagement.Models.Relations
{
    public class ProductCategory
    {
        public Guid ProductCategoryId { get; set; }

        public Guid ProductId { get; set; }

        public Guid CategoryId { get; set; }


        public Product Product { get; set; }

        public Category Category { get; set; }

        public ProductCategory()
        {
            ProductCategoryId = Guid.NewGuid();
            Product = null!;
            Category = null!;
        }
    }
}
