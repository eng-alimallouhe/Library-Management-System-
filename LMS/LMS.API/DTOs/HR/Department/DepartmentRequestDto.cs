using System.ComponentModel.DataAnnotations;

namespace LMS.API.DTOs.HR.Department
{
    public class DepartmentRequestDto
    {
        [Required]
        public string DepartmentName { get; set; }


        [Required]
        public string DepartmentDescription { get; set; }
    }
}