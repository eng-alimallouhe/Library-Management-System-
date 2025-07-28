using LMS.Application.Filters.Reports;

namespace LMS.Application.DTOs.Reports
{
    public class PaymentReportFilter : BaseReportFilter
    {
        public bool IncludeEmployee { get; set; } = true;
        public bool IncludeAmount { get; set; } = true;
        public bool IncludeDetails { get; set; } = true;
        public bool IncludeDate { get; set; } = true;
    }
}
