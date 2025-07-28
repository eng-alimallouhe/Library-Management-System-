namespace LMS.Application.DTOs.Admin.Dashboard
{
    public class TopSellingProductDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = default!;
        public decimal TotalSold { get; set; }
    }
}
