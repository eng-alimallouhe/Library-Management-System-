using LMS.Application.DTOs.Common;
using LMS.Domain.Identity.Models;

namespace LMS.Application.DTOs.Identity
{
    public class NotificationDto
    {
        public Guid NotificationId { get; set; }
        public required string Title { get; set; }
        public required string Message { get; set; }
        public string CreatedAt { get; set; }
        public bool IsRead { get; set; } = false;
        public string? RedirectUrl { get; set; }

        
    }
}
