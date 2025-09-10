namespace OrganizerPRO.Application.Common.Constants;

public static class UserCacheKeys
{
    private const string UserPrefix = "User";
    public static string UserContext(string userId) => $"{UserPrefix}:Context:{userId}";
    public static string UserProfile(string userId) => $"{UserPrefix}:Profile:{userId}";
    public static string UserApplication(string userId) => $"{UserPrefix}:Application:{userId}";
    public static string UserClaims(string userId) => $"{UserPrefix}:Claims:{userId}";
    public static string UserRoles(string userId) => $"{UserPrefix}:Roles:{userId}";
    public static string UserPermissions(string userId) => $"{UserPrefix}:Permissions:{userId}";
    public static string RoleClaims(string roleId) => $"Role:Claims:{roleId}";
    public static string[] AllUserKeys(string userId)
    {
        return
        [
            UserContext(userId),
            UserProfile(userId),
            UserApplication(userId),
            UserClaims(userId),
            UserRoles(userId),
            UserPermissions(userId)
        ];
    }

    public static string GetCacheKey(string userId, UserCacheType cacheType)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return string.Empty;

        return cacheType switch
        {
            UserCacheType.Context => UserContext(userId),
            UserCacheType.Profile => UserProfile(userId),
            UserCacheType.Application => UserApplication(userId),
            UserCacheType.Claims => UserClaims(userId),
            UserCacheType.Roles => UserRoles(userId),
            UserCacheType.Permissions => UserPermissions(userId),
            _ => string.Empty
        };
    }
}

public enum UserCacheType
{
    Context,
    Profile,
    Application,
    Claims,
    Roles,
    Permissions
}
