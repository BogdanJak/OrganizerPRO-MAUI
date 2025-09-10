namespace OrganizerPRO.Infrastructure.Services.MultiTenant;


public class MultiTenantUserStore : UserStore<
    ApplicationUser,
    ApplicationRole,
    ApplicationDbContext,
    string,
    ApplicationUserClaim,
    ApplicationUserRole,
    ApplicationUserLogin,
    ApplicationUserToken,
    ApplicationRoleClaim>
{

    public MultiTenantUserStore(ApplicationDbContext context)
        : base(context)
    {
    }


    public override async Task AddToRoleAsync(ApplicationUser user, string normalizedRoleName, CancellationToken cancellationToken = default)
    {
        // Check if the operation has been canceled
        cancellationToken.ThrowIfCancellationRequested();
        // Ensure the object has not been disposed before proceeding
        ThrowIfDisposed();

        // Validate the user and role name parameters
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (string.IsNullOrWhiteSpace(normalizedRoleName)) throw new ArgumentException("Value cannot be null or empty.", nameof(normalizedRoleName));

        // Retrieve the role entity for the given tenant and role name
        var roleEntity = await GetRoleAsync(normalizedRoleName, user.TenantId ?? string.Empty, cancellationToken);
        if (roleEntity == null) throw new InvalidOperationException($"Role '{normalizedRoleName}' does not exist in the user's tenant.");

        // Check if the user is already assigned to the role
        if (await IsUserInRoleAsync(user.Id, roleEntity.Id, cancellationToken)) return;

        // Add the user-role relationship to the context
        Context.UserRoles.Add(new ApplicationUserRole
        {
            UserId = user.Id,
            RoleId = roleEntity.Id
        });
    }

    public override async Task<bool> IsInRoleAsync(ApplicationUser user, string normalizedRoleName, CancellationToken cancellationToken = default)
    {
        // Check if the operation has been canceled
        cancellationToken.ThrowIfCancellationRequested();
        // Ensure the object has not been disposed before proceeding
        ThrowIfDisposed();

        // Validate the user and role name parameters
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (string.IsNullOrWhiteSpace(normalizedRoleName)) throw new ArgumentException("Value cannot be null or empty.", nameof(normalizedRoleName));

        // Retrieve the role entity for the given tenant and role name
        var role = await GetRoleAsync(normalizedRoleName, user.TenantId ?? string.Empty, cancellationToken);
        // Check if the user is in the role
        return role != null && await IsUserInRoleAsync(user.Id, role.Id, cancellationToken);
    }


    public override async Task RemoveFromRoleAsync(ApplicationUser user, string normalizedRoleName, CancellationToken cancellationToken = default)
    {
        // Check if the operation has been canceled
        cancellationToken.ThrowIfCancellationRequested();
        // Ensure the object has not been disposed before proceeding
        ThrowIfDisposed();

        // Validate the user and role name parameters
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (string.IsNullOrWhiteSpace(normalizedRoleName)) throw new ArgumentException("Value cannot be null or empty.", nameof(normalizedRoleName));

        // Retrieve the role entity for the given tenant and role name
        var role = await GetRoleAsync(normalizedRoleName, user.TenantId ?? string.Empty, cancellationToken);
        if (role != null)
        {
            // Retrieve the user-role relationship for the given user and role
            var userRole = await GetUserRoleAsync(user.Id, role.Id, cancellationToken);
            if (userRole != null)
            {
                // Remove the user-role relationship from the context
                Context.UserRoles.Remove(userRole);
            }
        }
    }

    private Task<ApplicationRole?> GetRoleAsync(string normalizedRoleName, string tenantId, CancellationToken cancellationToken)
    {
        return Context.Roles.FirstOrDefaultAsync(r => r.NormalizedName == normalizedRoleName && r.TenantId == tenantId, cancellationToken);
    }

    private Task<ApplicationUserRole?> GetUserRoleAsync(string userId, string roleId, CancellationToken cancellationToken)
    {
        return Context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);
    }

    private Task<bool> IsUserInRoleAsync(string userId, string roleId, CancellationToken cancellationToken)
    {
        return Context.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);
    }
}
