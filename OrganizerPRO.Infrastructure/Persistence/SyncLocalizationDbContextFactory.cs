namespace OrganizerPRO.Infrastructure.Persistence;


internal sealed class SyncLocalizationDbContextFactory(IDbContextFactory<SyncLocalizationContext> efFactory) : ISyncLocalizationDbContextFactory
{
    public ValueTask<ISyncLocalizationContext> CreateAsync(CancellationToken ct = default)
    {
        var dbContext = efFactory.CreateDbContext();
        return new ValueTask<ISyncLocalizationContext>(dbContext);
    }
}
