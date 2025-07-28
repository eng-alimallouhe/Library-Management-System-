using LMS.Application.Filters.Inventory;
using LMS.Domain.LibraryManagement.Models;
using LMS.Domain.LibraryManagement.Models.Products;

namespace LMS.Application.Abstractions.LibraryManagement
{
    public interface IInventoryHelper
    {
        Task<(ICollection<Product> items, int count)> GetInventorySnapshot(ProductFilter filter);

        Task<(ICollection<InventoryLog> items, int count)> GetInventoryLogsAsync(InventoryLogsFilter filter);

        Task<(ICollection<InventoryLog> items, int count)> GetLastInventoryLogsAsync(int Language);

        Task<(ICollection<Product> items, int count)> GetLowStockThresholdAsync(ProductFilter filter);
    }
}
