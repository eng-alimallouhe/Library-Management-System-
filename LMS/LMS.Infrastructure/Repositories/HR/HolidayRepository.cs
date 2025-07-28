using LMS.Domain.HR.Models;
using LMS.Infrastructure.DbContexts;

namespace LMS.Infrastructure.Repositories.HR
{
    public class HolidayRepository : BaseRepository<Holiday>
    {
        public HolidayRepository(LMSDbContext context) : base(context) { }
    }
}
