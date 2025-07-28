namespace LMS.Application.Filters.Reports
{
    public class BaseReportFilter
    {
        public string? Title { get; set; }

        public string? HeaderBackgroundColor { get; set; } = "#D1E7DD";
        public string? HeaderTextColor { get; set; } = "#000000";
        public string? BodyBackgroundColor { get; set; } = "#FFFFFF";
        public string? BodyTextColor { get; set; } = "#333333";
        public string? FontFamily { get; set; } = "Tajawal";

        public bool IncludeTotalRow { get; set; } = true;
        public int? HeaderFontSize { get; set; }
        public int? BodyFontSize { get; set; }
    }
}
