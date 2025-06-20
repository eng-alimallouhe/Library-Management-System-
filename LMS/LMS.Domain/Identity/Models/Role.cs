namespace LMS.Domain.Identity.Models
{
    public class Role
    {
        public Guid RoleId { get; set; }
        public required string RoleType { get; set; }
        public required string RoleDescription { get; set; }
    }
}
