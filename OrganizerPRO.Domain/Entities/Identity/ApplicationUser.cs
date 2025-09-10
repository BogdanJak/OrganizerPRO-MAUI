namespace OrganizerPRO.Domain.Entities.Identity;
public class ApplicationUser : IdentityUser, IAuditableEntity
{
    public ApplicationUser()
    {
        UserClaims = new HashSet<ApplicationUserClaim>();
        UserRoles = new HashSet<ApplicationUserRole>();
        Logins = new HashSet<ApplicationUserLogin>();
        Tokens = new HashSet<ApplicationUserToken>();
    }

    public string? FirstName { get; set; }
    public string? SurName { get; set; }
    public string? Gender { get; set; }
    public string? Designation { get; set; }
    public string? DisplayName { get; set; }
    public string? Provider { get; set; } = "Local";
    public string? TenantId { get; set; }
    public virtual Tenant? Tenant { get; set; }
    public string TenantName { get; set; }
    public string? ProfilePictureDataUrl { get; set; }
    public byte[]? ProfilePicture { get; set; }
    public bool IsActive { get; set; }
    public bool IsLive { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public virtual ICollection<ApplicationUserClaim> UserClaims { get; set; }
    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
    public virtual ICollection<ApplicationUserLogin> Logins { get; set; }
    public virtual ICollection<ApplicationUserToken> Tokens { get; set; }
    public string? SuperiorName { get; set; } = null!;
    public Guid? SuperiorParentId { get; set; } = null;
    public string? SuperiorUserId { get; set; } = null;
    public bool? HasChildren { get; set; }
    public string? OrgName { get; set; } = null;
    public Guid? SuperiorId { get; set; } = null;
    public ApplicationUser? Superior { get; set; } = null;
    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? CreatedById { get; set; } = null;
    public DateTime? LastModifiedAt { get; set; }
    public string? LastModifiedBy { get; set; }
    public string? LastModifiedById { get; set; } = null;

    public string? TimeZoneId { get; set; }
    public string? LanguageCode { get; set; }

}
