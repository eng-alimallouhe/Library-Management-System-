using LMS.Domain.LibraryManagement.Enums;
using LMS.Domain.LibraryManagement.Models;

namespace LMS.Application.DTOs.Stock
{
    public class InventoryLogOverviewDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string LogDate { get; set; }
        public LogType ChangeType { get; set; }
        public int ChangedQuantity { get; set; }
    }
}
