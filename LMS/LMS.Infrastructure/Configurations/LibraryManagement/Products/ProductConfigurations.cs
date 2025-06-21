using LMS.Domain.LibraryManagement.Models.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Configurations.LibraryManagement.Products
{
    public class ProductConfigurations :
        IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.Property(p => p.ProductPrice)
                    .HasColumnType("Decimal(19,2)")
                    .IsRequired();
        }
    }
}
