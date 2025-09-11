namespace OrganizerPRO.Application.Common.Interfaces;


public interface ISyncLocalizationDbContextFactory
{
    ValueTask<ISyncLocalizationDbContext> CreateAsync(CancellationToken ct = default);
}
