using LMS.Application.Filters.Finacial;
using LMS.Application.Filters.HR;
using LMS.Application.Filters.Inventory;
using LMS.Domain.Financial.Models;
using LMS.Domain.HR.Models;
using LMS.Domain.LibraryManagement.Models.Products;

namespace LMS.Application.Abstractions.Accounting
{
    public interface IFinancialHelper
    {
        Task<(ICollection<Revenue> items, int count)> GetFilteredRevenueAsync(RevenueFilter filter);
        Task<(ICollection<Payment> items, int count)> GetFilteredPaymentsAsync(PaymentFilter filter);
        Task<(ICollection<Product> items, int count)> GetDeadStockAsync(DeadStockFilter filter);
        Task<(ICollection<Salary> items, int count)> GetAllSalariesAsync(SalaryFilter filter);

    }
}
