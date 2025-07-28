using System.ComponentModel.DataAnnotations;

namespace LMS.API.DTOs.Authentication
{
    public class OtpCodeSendRequstDTO
    {
        [Required(ErrorMessage ="Email is required")]
        [EmailAddress(ErrorMessage = "Provide a valid email address")]
        public string Email { get; set; }

        


        [Range(0, 2, ErrorMessage = "Provide a valid Code Type")]
        [Required(ErrorMessage = "Code Type is required")]
        public int CodeType { get; set; }
    }
}
