using LMS.Application.Filters.Base;

namespace LMS.Application.Filters.Finacial
{
    public class PaymentFilter : Filter
    {
        public ICollection<Guid>? EmployeeIds { get; set; }
        public decimal? MaxAmount { get; set; }
        public decimal? MinAmount { get; set; }
        public bool IsDesc { get; set; } = false;
        public PaymentOrdering? Ordering { get; set; }
    }
}
