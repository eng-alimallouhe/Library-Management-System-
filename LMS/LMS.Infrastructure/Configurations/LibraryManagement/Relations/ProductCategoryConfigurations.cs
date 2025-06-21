using LMS.Domain.LibraryManagement.Models.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Configurations.LibraryManagement.Relations
{
    public class ProductCategoryConfigurations
        : IEntityTypeConfiguration<ProductCategory>
    {
        public void Configure(EntityTypeBuilder<ProductCategory> builder)
        {
            builder.ToTable("ProductsCategories");

            builder.HasKey(pc => pc.ProductCategoryId);
            
            builder.HasIndex(pc => pc.ProductId);
            
            builder.HasIndex(pc => pc.CategoryId);

            builder.HasOne(pc => pc.Category)
                .WithMany(c => c.ProductCategoriys)
                .HasForeignKey(pc => pc.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasOne(pc => pc.Product)
                .WithMany(p => p.ProductCategoriys)
                .HasForeignKey(pc => pc.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
