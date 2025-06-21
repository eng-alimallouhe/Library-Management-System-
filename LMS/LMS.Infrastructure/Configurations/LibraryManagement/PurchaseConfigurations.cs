using LMS.Domain.LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Configurations.LibraryManagement
{
    public class PurchaseConfigurations :
        IEntityTypeConfiguration<Purchase>
    {
        public void Configure(EntityTypeBuilder<Purchase> builder)
        {
            builder.ToTable("Purchases");

           
            builder.Property(p => p.TotalAmount)
                    .HasColumnType("Decimal(19,2)")
                    .IsRequired();
            
            builder.Property(p => p.CurrencyCode)
                    .HasMaxLength(3);

            
            builder.Property(p => p.Notes)
                    .HasMaxLength(255);
            
            
            builder.HasOne(p => p.Supplier)
                    .WithMany(s => s.Purchases)
                    .HasForeignKey(p => p.SupplierId)
                    .IsRequired();
        }
    }
}
