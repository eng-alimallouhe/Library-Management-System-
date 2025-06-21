using LMS.Domain.HR.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Configurations.HR
{
    public class PenaltyConfigurations :
        IEntityTypeConfiguration<Penalty>
    {
      public void Configure(EntityTypeBuilder<Penalty> builder)
        {
            builder.ToTable("Penalties");


            builder.Property(p => p.Amount)
                    .HasColumnType("decimal(19,2)")
                    .IsRequired();

            builder.Property(p => p.Reason)
                    .HasMaxLength(265)
                    .IsRequired();

            builder.Property(p => p.DecisionFileUrl)
                    .HasMaxLength(512)
                    .IsRequired();


            builder.HasOne(p => p.Employee)
                .WithMany(e => e.Penalties)
                .HasForeignKey(p => p.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        }
    }
}
