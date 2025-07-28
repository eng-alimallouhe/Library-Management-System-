using System.ComponentModel.DataAnnotations;

namespace LMS.API.DTOs.Authentication
{
    public class OtpVerifyRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Provide a valid email address")]
        public string Email { get; set; }


        [MaxLength(6, ErrorMessage = "Provide a valid langueage")]
        [MinLength(6, ErrorMessage = "Provide a valid langueage")]
        [Required(ErrorMessage = "Lnaguage is required")]
        public string Code { get; set; }
        
        

        [Required(ErrorMessage = "Code Type is required")]
        public int CodeType { get; set; }
    }
}
