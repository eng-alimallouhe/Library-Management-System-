using LMS.Domain.LibraryManagement.Models.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Configurations.LibraryManagement.Products
{
    public class ProductTranslationConfigurations :
        IEntityTypeConfiguration<ProductTranslation>
    {
        public void Configure(EntityTypeBuilder<ProductTranslation> builder)
        {
            builder.ToTable("ProductTranslations");

            builder.HasKey(a => a.TranslationId);

            builder.HasIndex(at => at.ProductId);

            builder.Property(a => a.ProductName)
                    .IsRequired()
                    .HasMaxLength(100);

            builder.Property(a => a.ProductDescription)
                    .IsRequired()
                    .HasMaxLength(512);

            builder.HasOne(at => at.Product)
                    .WithMany(a => a.Translations)
                    .HasForeignKey(at => at.ProductId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
        }
    }
}
