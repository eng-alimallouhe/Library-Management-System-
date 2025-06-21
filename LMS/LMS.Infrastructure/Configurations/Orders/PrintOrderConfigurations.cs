using LMS.Domain.Orders.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Configurations.Orders
{
    public class PrintOrderConfigurations :
        IEntityTypeConfiguration<PrintOrder>
    {
        public void Configure(EntityTypeBuilder<PrintOrder> builder)
        {
            builder.ToTable("PrintOrders");
            
            builder.Property(po => po.CopyCost)
                    .HasColumnType("decimal(15, 5)");
    
            builder.Property(po => po.FileUrl)
                    .HasMaxLength(512)
                    .IsRequired();
            
            builder.Property(po => po.FileName)
                    .HasMaxLength(256)
                    .IsRequired();
        }
    }
}
