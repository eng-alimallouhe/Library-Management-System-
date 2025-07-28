using LMS.Application.DTOs.Financial;
using LMS.Application.DTOs.Orders;
using LMS.Domain.Customers.Models;
using LMS.Domain.Identity.ValueObjects;

namespace LMS.Application.DTOs.Customers
{
    public class CustomerDetailsDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string LastLogIn { get; set; }
        public string CurrentLevel { get; set; }
        public decimal Points { get; set; }

        public ICollection<OrderOverviewDto> Orders { get; set; } = new List<OrderOverviewDto>();
        public ICollection<RevenueOverviewDto> Revenues { get; set; } = new List<RevenueOverviewDto>();
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
    }
}
