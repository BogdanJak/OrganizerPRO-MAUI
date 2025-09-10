namespace OrganizerPRO.Application.Common.Interfaces.Identity;

public interface IUserContextLoader
{
    Task<UserContext?> LoadAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default);
    void ClearUserContextCache(string userId);
} 