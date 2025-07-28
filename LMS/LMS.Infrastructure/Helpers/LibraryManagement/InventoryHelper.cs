using LMS.Application.Abstractions.LibraryManagement;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.Filters.Inventory;
using LMS.Domain.LibraryManagement.Models;
using LMS.Domain.LibraryManagement.Models.Products;
using LMS.Infrastructure.Specifications.LibraryManagement;

namespace LMS.Infrastructure.Helpers.LibraryManagement
{
    public class InventoryHelper : IInventoryHelper
    {
        private readonly ISoftDeletableRepository<Product> _ProductRepo;
        private readonly IBaseRepository<InventoryLog> _logsRepo;

        public InventoryHelper(
            ISoftDeletableRepository<Product> productRepo,
            IBaseRepository<InventoryLog> logsRepo)
        {
            _ProductRepo = productRepo;
            _logsRepo = logsRepo;
        }

        public async Task<(ICollection<InventoryLog> items, int count)> GetInventoryLogsAsync(InventoryLogsFilter filter)
        {
            var spec = new FilteredInventoryLogsSpecification(filter);

            return await _logsRepo.GetAllAsync(spec);
        }

        public async Task<(ICollection<Product> items, int count)> GetInventorySnapshot(ProductFilter filter)
        {
            var spec = new FilteredProductsSpecification(filter);

            var response = await _ProductRepo.GetAllAsync(spec);

            return response;
        }


        public async Task<(ICollection<Product> items, int count)> GetLowStockThresholdAsync(ProductFilter filter)
        {
            var spec = new FilteredProductsSpecification(filter);

            var response = await _ProductRepo.GetAllAsync(spec);

            return response;
        }
        
        public async Task<(ICollection<InventoryLog> items, int count)> GetLastInventoryLogsAsync(int Language)
        {
            var spec = new LastInventoryLogsSpecification(Language);

            return await _logsRepo.GetAllAsync(spec);
        }
    }
}