namespace OrganizerPRO.Application.Common.Interfaces;


public interface ISyncLocalizationDbContextFactory
{
    ValueTask<ISyncLocalizationContext> CreateAsync(CancellationToken ct = default);
}
