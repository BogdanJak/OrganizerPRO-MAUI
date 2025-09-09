namespace OrganizerPRO.Domain.Entities.Identity;
public class ApplicationUserRole : Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<string>
{
    public virtual ApplicationUser User { get; set; } = default!;
    public virtual ApplicationRole Role { get; set; } = default!;
}
