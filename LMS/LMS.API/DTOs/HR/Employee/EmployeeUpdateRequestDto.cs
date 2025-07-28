using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.DTOs.HR.Employee
{
    public class EmployeeUpdateRequestDto
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public decimal BaseSalary { get; set; }


        public IFormFile? EmployeeFaceImage { get; set; }
    }
}
