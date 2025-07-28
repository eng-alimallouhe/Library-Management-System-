using LMS.Domain.Customers.Models;

namespace LMS.Application.DTOs.Customers
{
    public class InActiveCustomersDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public decimal Posints { get; set; }
        public string Email { get; set; }
        public string CreatedAt { get; set; }
        public string LastOrderDate { get; set; }
    }
}