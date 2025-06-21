using LMS.Domain.LibraryManagement.Models;
using LMS.Infrastructure.DbContexts;

namespace LMS.Infrastructure.Repositories.LibraryManagement
{
    public class InventoryLogRepository : BaseRepository<InventoryLog>
    {
        public InventoryLogRepository(LMSDbContext context) : base(context) { }
    }
}
