using LMS.Domain.LibraryManagement.Models.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Configurations.LibraryManagement.Relations
{
    public class GenreBookConfigurations
        : IEntityTypeConfiguration<GenreBook>
    {
        public void Configure(EntityTypeBuilder<GenreBook> builder)
        {
            builder.ToTable("GenresBooks");

            builder.HasKey(pc => pc.GenreBookId);

            builder.HasOne(pc => pc.Book)
                .WithMany(c => c.GenreBooks)
                .HasForeignKey(pc => pc.BookId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasOne(pc => pc.Genre)
                .WithMany(p => p.GenreBooks)
                .HasForeignKey(pc => pc.GenreId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
