using LMS.Domain.HR.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Configurations.HR
{
    public class DepartmentConfigurations :
        IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("Departments");

            builder.HasKey(d => d.DepartmentId);
            
            builder.Property(d => d.DepartmentName)
                    .IsRequired()
                    .HasMaxLength(100);
            
            builder.Property(d => d.DepartmentDescription)
                    .IsRequired()
                    .HasMaxLength(512);
        }
    }
}
