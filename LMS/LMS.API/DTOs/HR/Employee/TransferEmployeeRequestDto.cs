using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.DTOs.HR.Employee
{
    public class TransferEmployeeRequestDto
    {
        [Required]
        public Guid EmployeeId { get; set; }

        [Required]
        public Guid DepartmentId { get; set; }

        [Required]
        [FromForm]
        public IFormFile AppointmentDesicionFile { get; set; }
    }
}
