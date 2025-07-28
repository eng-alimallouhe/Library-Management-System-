namespace LMS.Application.DTOs.Admin.Dashboard
{
    public class DashboardKpiDto
    {
        public int UsersCount { get; set; }
        public double UsersChangePercentage { get; set; }
        public int EmployeesCount { get; set; }
        public double EmployeesChangePercentage { get; set; }
        public int CustomersCount { get; set; }
        public double CustomersChangePercentage { get; set; }
        public int NewCustomersCount { get; set; }
        public int OrdersCount { get; set; }
        public double OrdersChangePercentage { get; set; }
        public int BooksCount { get; set; }
        public double BooksChangePercentage { get; set; }
        public int NewBooksCount { get; set; }
    }
}
