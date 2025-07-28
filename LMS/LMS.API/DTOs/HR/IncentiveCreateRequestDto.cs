using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.DTOs.HR
{
    public class IncentiveCreateRequestDto
    {
        [Required]
        public Guid EmployeeId { get; set; } = Guid.Empty;

        [Required]
        public decimal Amount { get; set; }


        [Required]
        [MaxLength(512)]
        public string Reason { get; set; }


        [FromForm]
        [Required]
        public IFormFile DecisionFile { get; set; }
    }
}
