namespace OrganizerPRO.Infrastructure.Services.Identity;


public class UsersStateContainer : IUsersStateContainer
{
    public ConcurrentDictionary<string, string> UsersByConnectionId { get; } = new();
    public event Action? OnChange;

    public void AddOrUpdate(string connectionId, string? name)
    {
        UsersByConnectionId.AddOrUpdate(connectionId, name ?? string.Empty, (key, oldValue) => name ?? string.Empty);
        NotifyStateChanged();
    }

    public void Remove(string connectionId)
    {
        UsersByConnectionId.TryRemove(connectionId, out _);
        NotifyStateChanged();
    }


    public void Clear(string userName)
    {
        var keysToRemove = UsersByConnectionId.Where(kvp => kvp.Value == userName).Select(kvp => kvp.Key).ToList();
        foreach (var key in keysToRemove)
        {
            UsersByConnectionId.TryRemove(key, out _);
        }
        NotifyStateChanged();
    }

    private void NotifyStateChanged()
    {
        OnChange?.Invoke();
    }
}

