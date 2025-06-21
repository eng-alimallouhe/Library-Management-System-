using LMS.Domain.Orders.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Configurations.Orders
{
    public class ShipmentConfigurations :
        IEntityTypeConfiguration<Shipment>
    {
        public void Configure(EntityTypeBuilder<Shipment> builder)
        {
            builder.ToTable("Shipment");

            builder.Property(s => s.Cost)
                .HasColumnType("DECIMAL(15,5)");

            builder.HasOne(dor => dor.Address)
                    .WithOne()
                    .HasForeignKey<Shipment>(s => s.AddressId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

            builder.HasOne(dor => dor.Order)
                    .WithOne()
                    .HasForeignKey<Shipment>(s => s.OrderId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();


        }
    }
}
