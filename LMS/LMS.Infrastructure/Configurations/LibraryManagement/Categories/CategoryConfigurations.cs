using LMS.Domain.LibraryManagement.Models.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Configurations.LibraryManagement.Categories
{
    public class CategoryConfigurations :
        IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");

            builder.HasKey(g => g.CategoryId);

            builder.Property(g => g.IsActive)
                    .IsRequired();
            
            builder.Property(g => g.CreatedAt)
                    .IsRequired();
            
            builder.Property(g => g.UpdatedAt)
                    .IsRequired();
        }
    }
}
