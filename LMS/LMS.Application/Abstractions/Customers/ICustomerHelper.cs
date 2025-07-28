using LMS.Application.Filters.Customers;
using LMS.Domain.Customers.Models;

namespace LMS.Application.Abstractions.Customers
{
    public interface ICustomerHelper
    {
        Task<(ICollection<Customer> items, int count)> GetAllCustomersAsync(CustomersFilter filter);

        Task<(ICollection<Customer> items, int count)> GetInActiveCustomersAsync(CustomersFilter filter);

        Task<Customer?> BuildCustomerResponseAsync(Guid id);
    }
}
