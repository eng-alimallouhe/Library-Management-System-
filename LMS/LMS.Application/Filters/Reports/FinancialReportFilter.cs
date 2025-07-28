using LMS.Application.DTOs.Reports;

namespace LMS.Application.Filters.Reports
{
    public class FinancialReportFilter : BaseReportFilter
    {
        //Shared Filters
        public bool IncludeEmployeeName { get; set; } = true;
        public bool IncludeAmount { get; set; } = true;
        public bool IncludeService { get; set; } = true;
        public bool IncludeDate { get; set; } = true;

        public string PaymentTitle { get; set; }
        public string RevenueTitle { get; set; }

        //Revenues Filters
        public bool IncludeCustomerName { get; set; } = true;


        //Payments Filters
        public bool IncludeDetails { get; set; } = true;
    }
}
