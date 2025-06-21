using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Domain.Orders.Models
{
    public class Order : BaseOrder
    {
        // Navigation property:
        public ICollection<OrderItem> OrderItems { get; set; }


        [NotMapped]
        public decimal TotalCost => OrderItems.Sum(o => o.TotalPrice);


        public Order()
        {
            OrderItems = new HashSet<OrderItem>();
        }
    }
}
