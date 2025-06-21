using LMS.Domain.HR.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Configurations.HR
{
    public class SalaryConfigurations :
        IEntityTypeConfiguration<Salary>
    {
        public void Configure(EntityTypeBuilder<Salary> builder)
        {
            builder.ToTable("Salaries");

            builder.Property(s => s.BaseSalary)
                    .HasColumnType("decimal(15,5)");

            builder.Property(s => s.TotalPenalties)
                    .HasColumnType("decimal(15,5)");


            builder.Property(s => s.TotalIncentives)
                    .HasColumnType("decimal(15,5)");



            builder.HasOne(s => s.Employee)
                .WithMany(e => e.Salaries)
                .HasForeignKey(s => s.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        }
    }
}
