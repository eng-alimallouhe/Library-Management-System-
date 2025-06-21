using LMS.Domain.Financial.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Configurations.Financial
{
    public class RevenueConfigurations :
        IEntityTypeConfiguration<Revenue>
    {
        public void Configure(EntityTypeBuilder<Revenue> builder)
        {
            builder.ToTable("Financials");

            builder.HasIndex(fr => fr.CustomerId);
            
            
            builder.HasIndex(fr => fr.EmployeeId);


            builder.Property(fr => fr.Amount)
                    .HasColumnType("Decimal(19,2)")
                    .IsRequired();
            

            
            builder.HasOne(fr => fr.Customer)
                .WithMany(c => c.Revenues)
                .HasForeignKey(fr => fr.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(fr => fr.Employee)
                .WithMany(e => e.Revenues)
                .HasForeignKey(fr => fr.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
