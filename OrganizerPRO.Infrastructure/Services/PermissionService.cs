using System;
using System.Collections.Generic;
using System.Text;

namespace OrganizerPRO.Infrastructure.Services;
public class PermissionService : IPermissionService
{
    private readonly IAuthorizationService _authService;
    private readonly AuthenticationStateProvider _authStateProvider;

    public PermissionService(IAuthorizationService authService, AuthenticationStateProvider authStateProvider)
    {
        _authService = authService;
        _authStateProvider = authStateProvider;
    }


    public async Task<bool> HasPermissionAsync(string permission)
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        var result = await _authService.AuthorizeAsync(user, permission);
        return result.Succeeded;
    }


    public async Task<List<string>> GetUserPermissionsAsync()
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        var permissions = new List<string>();
        var allPermissions = GetAllPermissions();

        // Check each permission for the current user
        var tasks = allPermissions.Select(async permission =>
        {
            var result = await _authService.AuthorizeAsync(user, permission);
            return new { Permission = permission, HasPermission = result.Succeeded };
        });

        var results = await Task.WhenAll(tasks);
        permissions.AddRange(results.Where(r => r.HasPermission).Select(r => r.Permission));

        return permissions;
    }


    public List<string> GetAllPermissions()
    {
        var permissions = new List<string>();
        var permissionsType = typeof(Permissions);

        // Get all nested types (permission classes like Products, Contacts, etc.)
        var nestedTypes = permissionsType.GetNestedTypes(BindingFlags.Public | BindingFlags.Static);

        foreach (var nestedType in nestedTypes)
        {
            // Get all constant string fields from each nested type
            var fields = nestedType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                                   .Where(f => f.IsLiteral && f.FieldType == typeof(string));

            foreach (var field in fields)
            {
                var value = field.GetValue(null) as string;
                if (!string.IsNullOrEmpty(value))
                {
                    permissions.Add(value);
                }
            }
        }

        return permissions;
    }


    public async Task<TAccessRights> GetAccessRightsAsync<TAccessRights>() where TAccessRights : new()
    {
        // Retrieve the current authentication state.
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        var accessRightsResult = new TAccessRights();

        // Ensure the type name ends with "AccessRights" (e.g., "ContactsAccessRights").
        var typeName = typeof(TAccessRights).Name;
        if (!typeName.EndsWith("AccessRights", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("TAccessRights type name must end with 'AccessRights'");
        }

        // Extract the section name from the type name (e.g., "Contacts" from "ContactsAccessRights").
        var sectionName = typeName.Substring(0, typeName.Length - "AccessRights".Length);

        // Get all public instance properties of TAccessRights.
        var properties = typeof(TAccessRights).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        // Create a dictionary to hold tasks for checking permissions concurrently.
        var tasks = new Dictionary<PropertyInfo, Task<AuthorizationResult>>();

        foreach (var prop in properties)
        {
            // Only process boolean properties that are writable.
            if (prop.PropertyType == typeof(bool) && prop.CanWrite)
            {
                // Construct the permission claim string, e.g., "Permissions.Contacts.Create".
                var permissionClaim = $"Permissions.{sectionName}.{prop.Name}";
                // Start the permission check task for the given claim.
                tasks[prop] = _authService.AuthorizeAsync(user, permissionClaim);
            }
        }

        // Wait for all permission checks to complete concurrently.
        await Task.WhenAll(tasks.Values);

        // Assign the results to the corresponding properties in the access rights model.
        foreach (var kvp in tasks)
        {
            var property = kvp.Key;
            var authResult = kvp.Value.Result;
            property.SetValue(accessRightsResult, authResult.Succeeded);
        }

        return accessRightsResult;
    }
}
