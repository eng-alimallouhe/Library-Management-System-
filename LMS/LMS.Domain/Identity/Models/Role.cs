namespace LMS.Domain.Identity.Models
{
    public class Role
    {
        public Guid RoleId { get; set; }
        public required string RoleType { get; set; }
        public required string RoleDescription { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Role()
        {
            RoleId = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
        }
    }
}
