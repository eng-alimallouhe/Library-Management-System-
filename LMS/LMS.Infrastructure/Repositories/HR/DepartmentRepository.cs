using LMS.Common.Exceptions;
using LMS.Domain.HR.Models;
using LMS.Infrastructure.DbContexts;

namespace LMS.Infrastructure.Repositories.HR
{
    public class DepartmentRepository : SoftDeletableRepository<Department>
    {
        private readonly LMSDbContext _context;
        public DepartmentRepository(LMSDbContext context) : base(context)
        {
            _context = context;
        }
        public override async Task SoftDeleteAsync(Guid id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department != null)
            {
                department.IsActive = false;
                _context.Departments.Update(department);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new EntityNotFoundException("Department not found");
            }
        }


    }
}
