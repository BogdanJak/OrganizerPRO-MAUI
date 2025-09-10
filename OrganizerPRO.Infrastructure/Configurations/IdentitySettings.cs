namespace OrganizerPRO.Infrastructure.Configurations;

public class IdentitySettings : IIdentitySettings
{
    public const string Key = nameof(IdentitySettings);
    public bool RequireDigit { get; set; } = true;
    public int RequiredLength { get; set; } = 6;
    public int MaxLength { get; set; } = 16;
    public bool RequireNonAlphanumeric { get; set; } = true;
    public bool RequireUpperCase { get; set; } = true;
    public bool RequireLowerCase { get; set; } = false;
    public int DefaultLockoutTimeSpan { get; set; } = 30;
}