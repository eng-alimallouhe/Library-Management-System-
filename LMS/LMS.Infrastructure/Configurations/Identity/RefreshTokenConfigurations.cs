using LMS.Domain.Customers.Models;
using LMS.Domain.Identity.Models;
using LMS.Domain.Identity.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Configurations.Users
{
    public class RefreshTokenConfigurations :
        IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");

            builder.HasKey(a => a.RefreshTokenId);

            builder.HasIndex(a => a.UserId);

            builder.HasOne<User>()
                    .WithOne(c => c.RefreshToken)
                    .HasForeignKey<RefreshToken>(a => a.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
        }
    }
}
