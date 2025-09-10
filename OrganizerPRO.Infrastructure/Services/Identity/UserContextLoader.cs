using System;
using System.Collections.Generic;
using System.Text;
using ZiggyCreatures.Caching.Fusion;

namespace OrganizerPRO.Infrastructure.Services.Identity;
public class UserContextLoader : IUserContextLoader
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IFusionCache _fusionCache;

    public UserContextLoader(IServiceScopeFactory scopeFactory, IFusionCache fusionCache)
    {
        _scopeFactory = scopeFactory;
        _fusionCache = fusionCache;
    }

    public async Task<UserContext?> LoadAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default)
    {
        if (principal?.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        var userId = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return null;
        }

        var cacheKey = UserCacheKeys.GetCacheKey(userId, UserCacheType.Context);

        return await _fusionCache.GetOrSetAsync(
            cacheKey,
            async _ =>
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    var user = await userManager.GetUserAsync(principal);
                    if (user == null)
                    {
                        return null;
                    }

                    var roles = await userManager.GetRolesAsync(user);

                    return new UserContext(
                        UserId: user.Id,
                        UserName: user.UserName ?? string.Empty,
                        DisplayName: user.DisplayName,
                        TenantId: user.TenantId,
                        Email: user.Email,
                        Roles: roles.ToList().AsReadOnly(),
                        ProfilePictureDataUrl: user.ProfilePictureDataUrl,
                        SuperiorId: user.SuperiorId.ToString()
                    );
                }
                catch (Exception)
                {
                    return null;
                }
            },
            options: new FusionCacheEntryOptions(TimeSpan.FromHours(1)),
            cancellationToken
        );
    }

    public void ClearUserContextCache(string userId)
    {
        if (!string.IsNullOrEmpty(userId))
        {
            var cacheKey = UserCacheKeys.GetCacheKey(userId, UserCacheType.Context);
            _fusionCache.Remove(cacheKey);
        }
    }
}
