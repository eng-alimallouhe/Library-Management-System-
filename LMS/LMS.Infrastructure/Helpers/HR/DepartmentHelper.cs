using AutoMapper;
using LMS.Application.Abstractions.HR;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.DTOs.HR.Departments;
using LMS.Application.DTOs.HR.Employees;
using LMS.Application.DTOs.Orders;
using LMS.Application.Filters.HR;
using LMS.Domain.HR.Models;
using LMS.Domain.Orders.Models;
using LMS.Infrastructure.Specifications.HR.EmployeesDepartments;

namespace LMS.Infrastructure.Helpers.HR
{
    public class DepartmentHelper : IDepartmentHelper
    {
        private readonly IMapper _mapper;
        private readonly ISoftDeletableRepository<Order> _baseOrderRepo;
        private readonly ISoftDeletableRepository<EmployeeDepartment> _employeeDepartmentRepo;
        private readonly ISoftDeletableRepository<Department> _departmentRepo;

        public DepartmentHelper(
            IMapper mapper,
            ISoftDeletableRepository<Order> baseOrderRepo,
            ISoftDeletableRepository<EmployeeDepartment> employeeDepartmentRepo,
            ISoftDeletableRepository<Department> departmentRepo)
        {
            _mapper = mapper;
            _baseOrderRepo = baseOrderRepo;
            _employeeDepartmentRepo = employeeDepartmentRepo;
            _departmentRepo = departmentRepo;
        }


        public async Task<DepartmentDetailsDTO> BuildDepartmentResponseAsync(Department department)
        {
            var departmentDetails = _mapper.Map<DepartmentDetailsDTO>(department);

            var empDeps = await _employeeDepartmentRepo.GetAllAsync(new EmployeeDepartmentByDepartmentIdSpecification(department.DepartmentId));


            var currentEmployeeDeps = empDeps.items.Where(ed => ed.IsActive).ToList();
            var formerEmployeeDeps = empDeps.items.Where(ed => !ed.IsActive).ToList();


            var currentEmployees = currentEmployeeDeps.Select(ed => ed.Employee).Distinct().ToList();
            var formerEmployees = formerEmployeeDeps.Select(ed => ed.Employee).Distinct().ToList();



            departmentDetails.CurrentEmployees = _mapper.Map<ICollection<EmployeeOverviewDto>>(currentEmployees);
            departmentDetails.FormerEmployees = _mapper.Map<ICollection<EmployeeOverviewDto>>(formerEmployees);



            var orders = await _baseOrderRepo.GetAllByExpressionAsync(bo => bo.DepartmentId == department.DepartmentId);

            departmentDetails.CurrentOrders = _mapper.Map<ICollection<OrderOverviewDto>>(orders);

            return departmentDetails;
        }

        // التعديل المقترح لـ GetDepartmentsAsync في DepartmentHelper
        // مع الافتراض أن لديك دالة لحساب العدد الإجمالي في المستودع
        public async Task<(ICollection<DepartmentOverviewDto> items, int count)> GetDepartmentsAsync(DepartmentFilter filter)
        {
            var spec = new DepartmentFilteredSpecification(filter);

            // افتراض: يجب أن يكون لديك دالة CountAsync في IBaseRepository أو ISoftDeletableRepository
            // Task<int> CountAsync(ISpecification<TEntity> spec);
            // أو Task<int> CountAsync(Expression<Func<TEntity, bool>>? criteria);
            // إذا كانت GetAllAsync هي الوحيدة التي تُرجع العدد، فيمكنك استخدامها كما هي،
            // ولكن ProjectTo ستكون أكثر كفاءة للجلب.
            // بما أن `ProjectToListAsync` لا تُرجع العدد، ستحتاج إلى جلب العدد بشكل منفصل.

            var totalCount = await _departmentRepo.GetCount(spec.Criteria!);

            var filteredDepartments = await _departmentRepo.ProjectToListAsync<DepartmentOverviewDto>(spec, _mapper.ConfigurationProvider);

            return (filteredDepartments, totalCount);
        }


        public async Task<ICollection<Department>> GetAvaliableDepartmentsAsync(Guid? employeeId)
        {
            var spec = new AvaliableDepartmentsSpecification(employeeId);

            var result = await _departmentRepo.GetAllAsync(spec);

            return result.items;
        }
    }
}