using LMS.Domain.Customers.Models.Levels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Configurations.Customers.Levels
{
    public class LevelConfigurations :
        IEntityTypeConfiguration<LoyaltyLevel>
    {
        public void Configure(EntityTypeBuilder<LoyaltyLevel> builder)
        {
            builder.ToTable("Levels");

            builder.HasKey(l => l.LevelId);
            
            
            builder.Property(l => l.DiscountPercentage)
                    .HasColumnType("Decimal(7,2)")
                    .IsRequired();

            builder.Property(l => l.PointPerDolar)
                   .HasColumnType("Decimal(7,2)")
                   .IsRequired();
        }
    }
}
