using LMS.Application.Abstractions.Customers;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.Filters.Customers;
using LMS.Domain.Customers.Models;
using LMS.Infrastructure.Specifications.Customers;

namespace LMS.Infrastructure.Helpers.Admin
{
    public class CustomerHelper : ICustomerHelper
    {
        private readonly ISoftDeletableRepository<Customer> _cutomerRepo;

        public CustomerHelper(
            ISoftDeletableRepository<Customer> cutomerRepo)
        {
            _cutomerRepo = cutomerRepo;
        }

        public async Task<Customer?> BuildCustomerResponseAsync(Guid id)
        {
            return await _cutomerRepo.GetBySpecificationAsync(new CustomerByIdSpecification(id));
        }

        public async Task<(ICollection<Customer> items, int count)> GetAllCustomersAsync(CustomersFilter filter)
        {
            var response = await _cutomerRepo.GetAllAsync(new FilteredCustomerSpecification(filter));
            return response;
        }

        public async Task<(ICollection<Customer> items, int count)> GetInActiveCustomersAsync(CustomersFilter filter)
        {
            return await _cutomerRepo.GetAllAsync(new InActiveCustomersSpecification(filter));
        }
    }
}
