using LMS.Common.Exceptions;
using LMS.Domain.Financial.Models;
using LMS.Infrastructure.DbContexts;

namespace LMS.Infrastructure.Repositories.Financial
{
    public class RevenueRepository : SoftDeletableRepository<Revenue>
    {
        private readonly LMSDbContext _context;
        public RevenueRepository(LMSDbContext context) : base(context)
        {
            _context = context;
        }
        public override async Task SoftDeleteAsync(Guid id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment is not null)
            {
                payment.IsActive = false;
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new EntityNotFoundException("Payment not found");
            }
        }
    }
}
