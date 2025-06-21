using LMS.Domain.HR.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Configurations.HR
{
    public class LeaveBalanceConfigurations :
        IEntityTypeConfiguration<LeaveBalance>
    {
        public void Configure(EntityTypeBuilder<LeaveBalance> builder)
        {
            builder.ToTable("LeavesBalances");

            builder.HasIndex(lb => lb.EmployeeId);

            builder.HasOne(lb => lb.Employee)
                .WithOne(e => e.LeaveBalance)
                .HasForeignKey<LeaveBalance>(lb => lb.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        }
    }
}
