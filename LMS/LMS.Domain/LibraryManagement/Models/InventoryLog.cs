using LMS.Domain.LibraryManagement.Enums;
using LMS.Domain.LibraryManagement.Models.Products;

namespace LMS.Domain.LibraryManagement.Models
{
    public class InventoryLog
    {
        // Primary key:
        public Guid InventoryLogId { get; set; }


        // Foreign key:
        public Guid ProductId { get; set; }


        public DateTime LogDate { get; set; }

        public LogType ChangeType { get; set; }
        public int ChangedQuantity { get; set; }


        public Product Product { get; set; }


        public InventoryLog()
        {
            InventoryLogId = Guid.NewGuid();
            Product = null!;
        }
    }

}
