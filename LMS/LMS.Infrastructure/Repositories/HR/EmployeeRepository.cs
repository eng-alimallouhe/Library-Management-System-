using LMS.Domain.HR.Models;
using LMS.Infrastructure.DbContexts;

namespace LMS.Infrastructure.Repositories.HR
{
    public class EmployeeRepository : SoftDeletableRepository<Employee>
    {
        private readonly LMSDbContext _context;

        public EmployeeRepository(LMSDbContext context) : base(context)
        {
            _context = context;
        }

        public override async Task SoftDeleteAsync(Guid id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                throw new Exception("Not found");
            }

            employee.IsDeleted = true;
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }
    }
}
