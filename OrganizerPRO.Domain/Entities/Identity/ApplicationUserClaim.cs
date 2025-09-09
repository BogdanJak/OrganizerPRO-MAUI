namespace OrganizerPRO.Domain.Entities.Identity;
public class ApplicationUserClaim : Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<string>
{
    public string? Description { get; set; }
    public virtual ApplicationUser User { get; set; } = default!;
}
