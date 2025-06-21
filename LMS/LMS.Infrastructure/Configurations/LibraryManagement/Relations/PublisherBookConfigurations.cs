using LMS.Domain.LibraryManagement.Models.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Configurations.LibraryManagement.Relations
{
    public class PublisherBookConfigurations
        : IEntityTypeConfiguration<PublisherBook>
    {
        public void Configure(EntityTypeBuilder<PublisherBook> builder)
        {
            builder.ToTable("PublishersBooks");

            builder.HasKey(pc => pc.PublisherBookId);

            builder.HasIndex(pc => pc.PublisherId);


            builder.HasIndex(pc => pc.BookId);

            builder.HasOne(pc => pc.Book)
                .WithMany(c => c.PublisherBooks)
                .HasForeignKey(pc => pc.BookId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasOne(pc => pc.Publisher)
                .WithMany(p => p.PublisherBooks)
                .HasForeignKey(pc => pc.PublisherId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
