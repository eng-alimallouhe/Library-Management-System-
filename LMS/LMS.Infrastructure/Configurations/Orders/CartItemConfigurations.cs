using LMS.Domain.Orders.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Configurations.Orders
{
    public class CartItemConfigurations :
        IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.ToTable("CartItems");

            builder.HasIndex(ci => ci.CartId);

            builder.Property(ci => ci.UnitPrice)
                    .HasColumnType("decimal(15, 5)");
           
           
            
            builder.HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId);
            
            builder.HasOne(ci => ci.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ci => ci.Discount)
                    .WithOne()
                    .HasForeignKey<CartItem>(ci => ci.DiscounttId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
