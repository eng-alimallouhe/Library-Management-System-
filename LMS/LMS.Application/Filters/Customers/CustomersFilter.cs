using LMS.Application.Filters.Base;

namespace LMS.Application.Filters.Customers
{
    public class CustomersFilter : Filter
    {
        public bool IsDesc { get; set; } = true;
        public CustomerOrderBy OrderBy { get; set; } = CustomerOrderBy.ByName;
    }
}
