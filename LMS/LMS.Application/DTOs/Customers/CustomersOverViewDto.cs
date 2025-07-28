namespace LMS.Application.DTOs.Customers
{
    public class CustomersOverViewDto
    {
        public Guid CustomerId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string CreatedAt { get; set; }       
        public string LastLogIn { get; set; }
        public decimal TotalAmountSpent { get; set; }
    }
}