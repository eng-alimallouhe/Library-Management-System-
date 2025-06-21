using LMS.Domain.HR.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Configurations.HR
{
    public class IncentiveConfigurations :
        IEntityTypeConfiguration<Incentive>
    {
        public void Configure(EntityTypeBuilder<Incentive> builder)
        {
            builder.ToTable("Incentives");

            
            builder.HasIndex(i => i.EmployeeId);

            builder.Property(i => i.Amount)
                    .HasColumnType("decimal(19,2)")
                    .IsRequired();

            builder.Property(i => i.Reason)
                    .HasMaxLength(265)
                    .IsRequired();

            builder.Property(i => i.DecisionFileUrl)
                    .HasMaxLength(512)
                    .IsRequired();



            builder.HasOne(i => i.Employee)
                .WithMany(e => e.Incentives)
                .HasForeignKey(i => i.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        }
    }
}
