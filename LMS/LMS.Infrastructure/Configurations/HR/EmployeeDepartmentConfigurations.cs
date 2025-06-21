using LMS.Domain.HR.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Configurations.HR
{
    public class EmployeeDepartmentConfigurations :
        IEntityTypeConfiguration<EmployeeDepartment>
    {
        public void Configure(EntityTypeBuilder<EmployeeDepartment> builder)
        {
            builder.ToTable("EmployeesDepartments");

            builder.HasIndex(ed => ed.DepartmentId);


            builder.Property(ed => ed.AppointmentDecisionUrl)
                    .IsRequired()
                    .HasMaxLength(512);


            builder.HasOne(ed => ed.Employee)
                    .WithMany(e => e.EmployeeDepartments)
                    .HasForeignKey(ed => ed.EmployeeId)
                    .IsRequired();

            builder.HasOne(ed => ed.Department)
                    .WithMany(d => d.EmployeeDepartments)
                    .HasForeignKey(ed => ed.DepartmentId)
                    .IsRequired();
        }
    }
}
