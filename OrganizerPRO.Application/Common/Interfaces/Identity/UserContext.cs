namespace OrganizerPRO.Application.Common.Interfaces.Identity;

public sealed record UserContext(
    string UserId,
    string UserName,
    string? DisplayName = null,
    string? TenantId = null,
    string? Email = null,
    IReadOnlyList<string>? Roles = null,
    string? ProfilePictureDataUrl = null,
    string? SuperiorId = null
); 