using System.Linq.Expressions;
using LMS.Application.Filters.Inventory;
using LMS.Domain.Abstractions.Specifications;
using LMS.Domain.LibraryManagement.Models;

namespace LMS.Infrastructure.Specifications.LibraryManagement
{
    public class FilteredInventoryLogsSpecification : ISpecification<InventoryLog>
    {
        private readonly InventoryLogsFilter _filter;

        public FilteredInventoryLogsSpecification(InventoryLogsFilter filter)
        {
            _filter = filter;
        }

        public Expression<Func<InventoryLog, bool>>? Criteria =>
            log =>
                (_filter.ProductId == null || _filter.ProductId.Any() || 
                _filter.ProductId.Contains(log.ProductId)) &&

                (_filter.From == null || log.LogDate >= _filter.From) &&
                (_filter.To == null || log.LogDate <= _filter.To) &&

                (_filter.ChangeType == null || _filter.ChangeType.Any() || 
                _filter.ChangeType.Contains((int) log.ChangeType));

        public List<string> Includes => new()
        {
            "Product.Translations"
        };

        public bool IsTrackingEnabled => false;

        public Expression<Func<InventoryLog, object>>? OrderBy => log => log.LogDate;

        public Expression<Func<InventoryLog, object>>? OrderByDescending => null;

        public int? Skip => _filter.PageNumber;

        public int? Take => _filter.PageSize;
    }
}
