using LMS.Domain.Financial.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Configurations.Financial
{
    public class PaymentConfigurations :
        IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");

            
            builder.HasIndex(p => p.EmployeeId);

            
            builder.Property(p => p.Amount)
                    .HasColumnType("decimal(19, 2)");


            builder.HasOne(p => p.Employee)
                .WithMany()
                .HasForeignKey(p => p.EmployeeId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
