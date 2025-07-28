using LMS.Application.DTOs.HR.Employees;
using LMS.Application.DTOs.Orders;

namespace LMS.Application.DTOs.HR.Departments
{
    public class DepartmentDetailsDTO
    {
        public string DepartmentName { get; set; }= string.Empty;
        public string DepartmentDescription { get; set; } = string.Empty;

        public ICollection<EmployeeOverviewDto> CurrentEmployees { get; set; } = [];
        public ICollection<EmployeeOverviewDto> FormerEmployees { get; set; } = [];
        public ICollection<OrderOverviewDto> CurrentOrders { get; set; } = [];
    }
}