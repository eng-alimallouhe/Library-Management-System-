using LMS.Domain.HR.Models;
using LMS.Infrastructure.DbContexts;

namespace LMS.Infrastructure.Repositories.HR
{
    public class LeaveBalanceRepository : BaseRepository<LeaveBalance>
    {
        public LeaveBalanceRepository(LMSDbContext context) : base(context) { }
    }

}
