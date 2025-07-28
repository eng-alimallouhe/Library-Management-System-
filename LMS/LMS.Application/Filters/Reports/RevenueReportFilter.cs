namespace LMS.Application.Filters.Reports
{
    public class RevenueReportFilter : BaseReportFilter
    {
        // الفلاتر العامة
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public Guid? EmployeeId { get; set; }
        public Guid? CustomerId { get; set; }

        // الأعمدة المطلوبة
        public bool IncludeEmployeeName { get; set; } = true;
        public bool IncludeCustomerName { get; set; } = true;
        public bool IncludeAmount { get; set; } = true;
        public bool IncludeService { get; set; } = true;
        public bool IncludeDate { get; set; } = true;
    }
}
