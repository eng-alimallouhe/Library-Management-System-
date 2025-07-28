using LMS.Application.Filters.Base;
using LMS.Domain.Financial.Enums;

namespace LMS.Application.Filters.Finacial
{
    public class RevenueFilter : Filter
    {
        public ICollection<Guid>? CustomerIds { get; set; }
        public ICollection<Guid>? EmployeeIds { get; set; }
        public Service? Service { get; set; }
        public decimal? MaxAmount { get; set; }
        public decimal? MinAmount { get; set; }
        public RevenueOrdering? OrderBy { get; set; }
        public bool Desc { get; set; } = true;
    }
}