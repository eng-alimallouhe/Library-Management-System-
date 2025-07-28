using System.Linq.Expressions;
using LMS.Domain.Abstractions.Specifications;
using LMS.Domain.Identity.Enums;
using LMS.Domain.LibraryManagement.Models;

namespace LMS.Infrastructure.Specifications.LibraryManagement
{
    public class LastInventoryLogsSpecification : ISpecification<InventoryLog>
    {
        private readonly int _language;

        public Expression<Func<InventoryLog, bool>>? Criteria => 
            log => log.Product.Translations
            .Any(pt => pt.Language == (Language) _language);

        public List<string> Includes => ["Product.Translations"];

        public bool IsTrackingEnabled => false;

        public Expression<Func<InventoryLog, object>>? OrderBy => log => log.LogDate;

        public Expression<Func<InventoryLog, object>>? OrderByDescending => null;

        public int? Skip => null;

        public int? Take => 10;

        public LastInventoryLogsSpecification(
            int language)
        {
            _language = language;
        }
    }
}
