using LMS.Common.Exceptions;
using LMS.Domain.Identity.Models;
using LMS.Infrastructure.DbContexts;

namespace LMS.Infrastructure.Repositories.Identity
{
    public class UserRepositroy : SoftDeletableRepository<User>
    {
        private readonly LMSDbContext _context;

        public UserRepositroy(LMSDbContext context) : base(context)
        {
            _context = context;
        }

        public override async Task SoftDeleteAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.IsDeleted = true;
                await _context.SaveChangesAsync();
                return;
            }
            throw new EntityNotFoundException("user not found");
        }
    }
}
