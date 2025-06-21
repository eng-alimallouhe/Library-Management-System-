using LMS.Domain.LibraryManagement.Models.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Configurations.LibraryManagement.Categories
{
    public class CategoryTranslationConfigurations :
        IEntityTypeConfiguration<CategoryTranslation>
    {
        public void Configure(EntityTypeBuilder<CategoryTranslation> builder)
        {
            builder.ToTable("CategoryTranslations");

            builder.HasKey(a => a.TranslationId);

            builder.HasIndex(at => at.CategoryId);

            builder.Property(a => a.CategoryName)
                    .IsRequired()
                    .HasMaxLength(100);

            builder.Property(a => a.CategoryDescription)
                    .IsRequired()
                    .HasMaxLength(512);

            builder.HasOne(at => at.Category)
                    .WithMany(a => a.Translations)
                    .HasForeignKey(at => at.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
        }
    }
}
