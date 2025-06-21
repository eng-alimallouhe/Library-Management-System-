﻿using LMS.Domain.Customers.Models;
using LMS.Domain.Identity.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Configurations.Users
{
    public class AddressConfigurations :
        IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("Addresses");

            builder.HasKey(a => a.AddressId);
           
            builder.Property(a => a.Latitude)
                    .IsRequired()
                    .HasMaxLength(100);
            
            builder.Property(a => a.Longitude)
                    .IsRequired()
                    .HasMaxLength(100);
            
            builder.Property(a => a.Governorate)
                    .IsRequired()
                    .HasMaxLength(100);

            builder.Property(a => a.City)
                    .IsRequired()
                    .HasMaxLength(100);

            builder.Property(a => a.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(25);


            builder.Property(a => a.Details)
                    .IsRequired()
                    .HasMaxLength(200);


            builder.HasOne<Customer>()
                    .WithMany(c => c.Addresses)
                    .HasForeignKey(a => a.CustomerId)
                    .IsRequired(false);
        }
    }
}
