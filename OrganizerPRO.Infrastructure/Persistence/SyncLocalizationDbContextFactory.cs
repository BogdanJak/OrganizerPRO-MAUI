namespace OrganizerPRO.Infrastructure.Persistence;


internal sealed class SyncLocalizationDbContextFactory(IDbContextFactory<SyncLocalizationDbContext> efFactory) : ISyncLocalizationDbContextFactory
{
    public ValueTask<ISyncLocalizationDbContext> CreateAsync(CancellationToken ct = default)
    {
        var dbContext = efFactory.CreateDbContext();
        return new ValueTask<ISyncLocalizationDbContext>(dbContext);
    }
}
