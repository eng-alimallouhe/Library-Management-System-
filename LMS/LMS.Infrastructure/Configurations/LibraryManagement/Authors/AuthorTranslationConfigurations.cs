using LMS.Domain.LibraryManagement.Models.Authors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Configurations.LibraryManagement.Authors
{
    public class AuthorTranslationConfigurations :
        IEntityTypeConfiguration<AuthorTranslation>
    {
        public void Configure(EntityTypeBuilder<AuthorTranslation> builder)
        {
            builder.ToTable("AuthorTranslations");

            builder.HasKey(a => a.TranslationId);

            builder.HasIndex(at => at.AuthorId);

            builder.Property(a => a.AuthorName)
                    .IsRequired()
                    .HasMaxLength(100);
            
            builder.Property(a => a.AuthorDescription)
                    .IsRequired()
                    .HasMaxLength(512);
            
            builder.HasOne(at => at.Author)
                    .WithMany(a => a.Translations)
                    .HasForeignKey(at => at.AuthorId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
        }
    }
}
