namespace OrganizerPRO.Infrastructure.Services;


public class MemoryCacheTicketStore : ITicketStore
{
    private const string KeyPrefix = "AuthSessionStore-";
    private readonly IFusionCache _cache;

    public MemoryCacheTicketStore()
    {
        _cache = new FusionCache(new FusionCacheOptions()
        {
            CacheName = "AuthSessionStore",
            DefaultEntryOptions = new FusionCacheEntryOptions
            {
                Duration = TimeSpan.FromDays(15),
                // FAIL-SAFE OPTIONS
                IsFailSafeEnabled = true,
                FailSafeMaxDuration = TimeSpan.FromHours(2),
                FailSafeThrottleDuration = TimeSpan.FromSeconds(30),
                // FACTORY TIMEOUTS
                FactorySoftTimeout = TimeSpan.FromMilliseconds(300),
                FactoryHardTimeout = TimeSpan.FromMilliseconds(1500)
            }
        });
    }

    public async Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        var guid = Guid.NewGuid();
        var key = KeyPrefix + guid.ToString();
        await RenewAsync(key, ticket);
        return key;
    }

    public async Task RenewAsync(string key, AuthenticationTicket ticket)
    {
        await _cache.SetAsync(key, ticket);
    }

    public async Task<AuthenticationTicket> RetrieveAsync(string key)
    {
        var ticket = await _cache.GetOrDefaultAsync<AuthenticationTicket>(key);
        return ticket;
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }
}
