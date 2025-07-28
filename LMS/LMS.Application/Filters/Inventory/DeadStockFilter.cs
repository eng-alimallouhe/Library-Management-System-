using LMS.Application.Filters.Base;

namespace LMS.Application.Filters.Inventory
{
    public class DeadStockFilter : Filter
    {
        public bool IsDesc { get; set; } = false;
        public DeadStockOrdering? OrderBy { get; set; }
    }
}