﻿using System.ComponentModel.DataAnnotations.Schema;
using LMS.Domain.Customers.Models;
using LMS.Domain.HR.Models;
using LMS.Domain.Orders.Enums;

namespace LMS.Domain.Orders.Models
{
    public class Order
    {
        //primary key:
        public Guid OrderId { get; set; }


        //Foreign Key: CustomerId ==> one(Customer)-to-many(Order) relationship
        public Guid CustomerId { get; set; }


        //Foreign Key: EmployeeId ==> one(Employee)-to-many(Order) relationship
        public Guid? EmployeeId { get; set; }


        //Foreign Key: EmployeeId ==> one(Employee)-to-many(Order) relationship
        public Guid DepartmentId { get; set; }


        public OrderType OrderType { get; set; }

        public OrderStatus Status { get; set; }
        public DeliveryMethod DeliveryMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public decimal Cost { get; set; }


        //Soft delete:
        public bool IsActive { get; set; }


        //Timestamp:
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


        //nabigation property:
        public Customer Customer { get; set; }
        public Employee? Employee { get; set; }
        public Department Department { get; set; }



        // Navigation property:
        public ICollection<OrderItem> OrderItems { get; set; }


        [NotMapped]
        public decimal TotalCost => OrderItems.Sum(o => o.TotalPrice);
        public Order()
        {
            OrderId = Guid.NewGuid();
            IsActive = true;
            Status = OrderStatus.Pending;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            Customer = null!;
            Department = null!;
            OrderItems = new HashSet<OrderItem>();
        }
    }
}
