using LMS.Domain.Customers.Models.Levels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Configurations.Customers.Levels
{
    public class LevelTranslationConfigurations :
        IEntityTypeConfiguration<LoyaltyLevelTransaltion>
    {
        public void Configure(EntityTypeBuilder<LoyaltyLevelTransaltion> builder)
        {
            builder.ToTable("LevelTranslation");

            builder.HasKey(l => l.TranslationId);
            
            builder.HasIndex(l => l.LevelId);


            builder.Property(l => l.LevelName)
                    .IsRequired()
                    .HasMaxLength(100);
           
            builder.Property(l => l.LevelDescription)
                    .IsRequired()
                    .HasMaxLength(255);


            builder.HasOne(lt => lt.LoyaltyLevel)
                    .WithMany(l => l.Translations)
                    .HasForeignKey(lt => lt.LevelId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
        }
    }
}
