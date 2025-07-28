namespace LMS.Application.Filters.Reports
{
    public class StockReportFilter : BaseReportFilter
    {
        public bool IncludeProductId { get; set; } = true;
        public bool IncludeProductName { get; set; } = true;
        public bool IncludeProductStock { get; set; } = true;
        public bool IncludeProductPrice { get; set; } = true;
        public bool IncludeTotalValue { get; set; } = true;
        public bool IncludeUpdatedAt { get; set; } = true;
        public bool IncludeLogsCount { get; set; } = true;
        public bool IncludeQuantity { get; set; } = true;
    }
}