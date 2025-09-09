namespace OrganizerPRO.Domain.Entities.Identity;
public class ApplicationRole : Microsoft.AspNet.Identity.EntityFramework.IdentityRole, IAuditableEntity
{
    public ApplicationRole()
    {
        RoleClaims = new HashSet<ApplicationRoleClaim>();
        UserRoles = new HashSet<ApplicationUserRole>();
    }

    public ApplicationRole(string roleName) : base(roleName)
    {
        RoleClaims = new HashSet<ApplicationRoleClaim>();
        UserRoles = new HashSet<ApplicationUserRole>();
    }

    public string? TenantId { get; set; }
    public virtual Tenant? Tenant { get; set; }
    public string? Description { get; set; }
    public virtual ICollection<ApplicationRoleClaim> RoleClaims { get; set; }
    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? CreatedById { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public string? LastModifiedById { get; set; }
}
