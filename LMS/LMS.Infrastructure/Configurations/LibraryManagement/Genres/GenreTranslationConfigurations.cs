using LMS.Domain.LibraryManagement.Models.Genres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Configurations.LibraryManagement.Genres
{
    public class GenreTranslationConfigurations :
        IEntityTypeConfiguration<GenreTranslation>
    {
        public void Configure(EntityTypeBuilder<GenreTranslation> builder)
        {
            builder.ToTable("GenreTranslations");

            builder.HasKey(a => a.TranslationId);

            builder.HasIndex(at => at.GenreId);


            builder.Property(a => a.GenreName)
                    .IsRequired()
                    .HasMaxLength(100);


            builder.Property(a => a.GenreDescription)
                    .IsRequired()
                    .HasMaxLength(512);


            builder.HasOne(at => at.Genre)
                    .WithMany(a => a.Translations)
                    .HasForeignKey(at => at.GenreId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
        }
    }
}
