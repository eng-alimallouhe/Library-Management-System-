namespace LMS.Application.DTOs.Admin.Dashboard
{
    public class UsersCountDto
    {
        public int UsersCount { get; set; }
        public double UsersChangePercentage { get; set; }

        public int EmployeesCount { get; set; }
        public double EmployeesChangePercentage { get; set; }

        public int CustomersCount { get; set; }
        public double CustomersChangePercentage { get; set; }
        public int NewCustomersCount { get; set; }
    }
}
