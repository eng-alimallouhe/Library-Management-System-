namespace LMS.Application.DTOs.Stock
{
    public class DeadStockDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int ProductStock { get; set; }
        public decimal ProductPrice { get; set; }
        public string LastMovementDate { get; set; }
        public string CreatedAt { get; set; }
    }
}