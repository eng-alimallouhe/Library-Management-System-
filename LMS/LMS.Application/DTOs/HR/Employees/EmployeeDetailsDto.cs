using LMS.Application.DTOs.Financial;
using LMS.Application.DTOs.HR.Departments;
using LMS.Domain.HR.Models;

namespace LMS.Application.DTOs.HR.Employees
{
    public class EmployeeDetailsDto
    {
        public Guid EmployeeId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
        public string Email { get; set; } = string.Empty;
        public decimal BaseSalary { get; set; }
        public string? CurrentDepartmentName { get; set; } = string.Empty;

        public ICollection<RevenueOverviewDto> EmployeeFinanical { get; set; } = [];
        public ICollection<DepartmentHistoryDto> DepartmentsHistory { get; set; } = [];
        public ICollection<AttendanceLookup> Attendances { get; set; } = [];
        public ICollection<IncentivesLookupDto>  Incentives { get; set; } = [];
        public ICollection<PenaltyLookupDto>  Penalties { get; set; } = [];
        public ICollection<LeaveOverviewDto>  Leaves { get; set; } = [];
        public ICollection<SalariesOverviewDto>  Salaries { get; set; } = [];
    }
}