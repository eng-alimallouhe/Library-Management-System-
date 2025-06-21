using LMS.Domain.LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Configurations.LibraryManagement
{
    public class SupplierConfigurations :
        IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.ToTable("Suppliers");

            builder.Property(s => s.SupplierName)
                    .IsRequired()
                    .HasMaxLength(100);

            builder.Property(s => s.ContactEmail)
                    .IsRequired()
                    .HasMaxLength(255);

            builder.Property(s => s.ContactPhone)
                    .IsRequired()
                    .HasMaxLength(23);
        }
    }
}
