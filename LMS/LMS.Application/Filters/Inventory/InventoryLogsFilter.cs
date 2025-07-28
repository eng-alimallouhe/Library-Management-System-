using LMS.Application.Filters.Base;

namespace LMS.Application.Filters.Inventory
{
    public class InventoryLogsFilter
    {
        public ICollection<int>? ChangeType { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public ICollection<Guid>? ProductId { get; set; }
    }
}
