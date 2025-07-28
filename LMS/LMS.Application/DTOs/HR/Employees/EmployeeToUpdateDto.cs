namespace LMS.Application.DTOs.HR.Employees
{
    public class EmployeeUpdateDto
    {
        public string FullName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public decimal BaseSalary { get; set; }
    }
}
