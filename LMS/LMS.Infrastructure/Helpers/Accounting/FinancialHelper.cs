using LMS.Application.Abstractions.Accounting;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.Filters.Finacial;
using LMS.Application.Filters.HR;
using LMS.Application.Filters.Inventory;
using LMS.Domain.Financial.Models;
using LMS.Domain.HR.Models;
using LMS.Domain.LibraryManagement.Models.Products;
using LMS.Infrastructure.Specifications.Finacial;
using LMS.Infrastructure.Specifications.HR;

namespace LMS.Infrastructure.Helpers.Accounting
{
    public class FinancialHelper : IFinancialHelper
    {
        private readonly ISoftDeletableRepository<Revenue> _revenueRepo;
        private readonly ISoftDeletableRepository<Payment> _paymentRepo;
        private readonly ISoftDeletableRepository<Product> _productRepo;
        private readonly ISoftDeletableRepository<Salary> _salaryRepo;

        public FinancialHelper(
            ISoftDeletableRepository<Revenue> revenueRepo,
            ISoftDeletableRepository<Payment> paymentRepo,
            ISoftDeletableRepository<Product> productRepo,
            ISoftDeletableRepository<Salary> salaryRepo)
        {
            _revenueRepo = revenueRepo; 
            _paymentRepo = paymentRepo;
            _productRepo = productRepo;
            _salaryRepo = salaryRepo;
        }

        public async Task<(ICollection<Product> items, int count)> GetDeadStockAsync(DeadStockFilter filter)
        {
            var spec = new FilteredDeadStockSpecification(filter);

            return await _productRepo.GetAllAsync(spec);
        }

        public async Task<(ICollection<Payment> items, int count)> GetFilteredPaymentsAsync(PaymentFilter filter)
        {
            var spec = new FilteredPaymentsSpecification(filter);

            return await _paymentRepo.GetAllAsync(spec); 
        }

        public async Task<(ICollection<Revenue> items, int count)> GetFilteredRevenueAsync(RevenueFilter filter)
        {
            var spec = new FilteredRevenueSpecification(filter);

            return await _revenueRepo.GetAllAsync(spec);
        }

        public async Task<(ICollection<Salary> items, int count)> GetAllSalariesAsync(SalaryFilter filter)
        {
            var spec = new FilteredSalariesSpecification(filter);
            return await _salaryRepo.GetAllAsync(spec);
        }
    }
}
