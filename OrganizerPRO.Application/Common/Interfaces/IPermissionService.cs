namespace OrganizerPRO.Application.Common.Interfaces;

public interface IPermissionService
{
    Task<bool> HasPermissionAsync(string permission);
    Task<T> GetAccessRightsAsync<T>() where T : new();
    Task<List<string>> GetUserPermissionsAsync();
    List<string> GetAllPermissions();
}
