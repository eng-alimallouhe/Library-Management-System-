using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.DTOs.HR
{
    public class CheckDto
    {
        [Required]
        public string UserName { get; set; }

        [FromForm]
        public IFormFile FaceImage { get; set; }
    }
}
