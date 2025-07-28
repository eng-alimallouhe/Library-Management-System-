using LMS.Domain.HR.Models;

namespace LMS.Application.DTOs.HR
{
    public class SalariesOverviewDto
    {
        public decimal BaseSalary { get; set; }
        public decimal TotalIncentives { get; set; }
        public decimal TotalPenalties { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}