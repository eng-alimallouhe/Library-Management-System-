using LMS.Application.Filters.Base;

namespace LMS.Application.Filters.Inventory
{
    public class ProductFilter : Filter
    {
        public decimal? MaxPrice { get; set; }
        public decimal? MinPrice { get; set; }
        public ICollection<Guid>? CategoryId { get; set; }
        public decimal? MaxQuantity { get; set; }
        public decimal? MinQuantity { get; set; }
    }
}