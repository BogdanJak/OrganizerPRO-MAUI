namespace OrganizerPRO.Domain.Entities.Identity;
public class ApplicationUserLogin : Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<string>
{
    public virtual ApplicationUser User { get; set; } = default!;
}
