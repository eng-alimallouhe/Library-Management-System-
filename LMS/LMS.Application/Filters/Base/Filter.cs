namespace LMS.Application.Filters.Base
{
    public class Filter
    {
        public string? Name { get; set; }
        public int Language { get; set; } = 0;
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public bool OnlyActiveRegisters { get; set; } = true;
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
}