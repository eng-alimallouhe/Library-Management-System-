namespace LMS.Application.Filters.Reports
{
    public class DeadStockReportFilter : BaseReportFilter
    {
        public bool IncludeProductStock { get; set; }
        public bool IncludeProductPrice { get; set; }
        public bool IncludeLastMovementDate { get; set; }
        public bool IncludeCreatedAt { get; set; }
    }
}
