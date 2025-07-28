using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.DTOs.HR
{
    public class PenaltyUpdateRequestDto
    {
        [Required]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(512)]
        public string Reason { get; set; } = string.Empty;


        [FromForm]
        public IFormFile? Decision { get; set; }
    }
}
