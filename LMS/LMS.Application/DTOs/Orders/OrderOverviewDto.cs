using LMS.Domain.Orders.Enums;

namespace LMS.Application.DTOs.Orders
{
    public class OrderOverviewDto
    {
        public Guid OrderId { get; set; }
        public OrderStatus Status { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }

        public decimal Cost { get; set; }
    }
}