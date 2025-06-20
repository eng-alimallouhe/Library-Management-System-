﻿using System.ComponentModel.DataAnnotations.Schema;
using LMS.Domain.LibraryManagement.Models;
using LMS.Domain.LibraryManagement.Models.Products;

namespace LMS.Domain.Orders.Models
{
    public class OrderItem
    {
        // Primary key:
        public Guid OrderItemId { get; set; }

        //Foreign Key: SellOrderId ==> one(SellOrder)-to-many(OrderItem) relationship
        public Guid SellOrderId { get; set; }

        //Foreign Key: ProductId ==> one(Product)-to-many(OrderItem) relationship
        public Guid ProductId { get; set; }


        //Foreign Key: DiscounttId ==> one(discount)-to-one(orderItem) relationship
        public Guid? DiscountId { get; set; }


        public int Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; }

        [NotMapped]
        public decimal DiscountAmount => Discount != null
            ? Quantity * UnitPrice * (Discount.DiscountPercentage / 100)
            : 0;

        [NotMapped]
        public decimal TotalPrice => (Quantity * UnitPrice) - DiscountAmount;

        
        //Soft Delete:
        public bool IsActive { get; set; }

        
        //Timestamp:
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


        // Navigation property:
        public Order Order { get; set; }
        public Product Product { get; set; }
        public Discount? Discount { get; set; }

        public OrderItem()
        {
            OrderItemId = Guid.NewGuid();
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            Order = null!;
            Product = null!;
        }
    }
}