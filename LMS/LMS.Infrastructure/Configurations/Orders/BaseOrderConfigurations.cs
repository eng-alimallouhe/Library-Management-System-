using LMS.Domain.Orders.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Configurations.Orders
{
    public class BaseOrderConfigurations :
        IEntityTypeConfiguration<BaseOrder>
    {
        public void Configure(EntityTypeBuilder<BaseOrder> builder)
        {
            builder.ToTable("BaseOrders");

            builder.HasKey(bo => bo.OrderId);

            builder.HasIndex(bo => bo.CustomerId);

            builder.HasIndex(bo => bo.EmployeeId);
            
            builder.HasIndex(bo => bo.DepartmentId);


            builder.HasOne(bo => bo.Department)
                    .WithMany()
                    .HasForeignKey(bo => bo.DepartmentId)
                    .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(bo => bo.Employee)
                    .WithMany(e => e.Orders)
                    .HasForeignKey( bo => bo.EmployeeId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(bo => bo.Customer)
                    .WithMany(e => e.Orders)
                    .HasForeignKey(bo => bo.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
