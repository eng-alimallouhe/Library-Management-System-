using LMS.Domain.Orders.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Configurations.Orders
{
    public class OrderItemConfigurations :
        IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems");


            builder.HasIndex(oi => oi.SellOrderId);
            
            builder.Property(oi => oi.UnitPrice)
                    .HasColumnType("decimal(15, 5)");
            
            
            builder.HasOne(oi => oi.Order)
                .WithMany(so => so.OrderItems)
                .HasForeignKey(oi => oi.SellOrderId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasOne(oi => oi.Product)
                    .WithMany()
                    .HasForeignKey(oi => oi.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
