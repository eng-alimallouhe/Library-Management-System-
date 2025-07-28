namespace LMS.Application.DTOs.Authentication
{
    public class AuthorizationDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
