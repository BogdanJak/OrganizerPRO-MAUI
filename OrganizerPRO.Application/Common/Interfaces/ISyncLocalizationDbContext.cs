namespace OrganizerPRO.Application.Common.Interfaces;


public interface ISyncLocalizationDbContext : IAsyncDisposable
{
    DbSet<Localizer> Localizers { get; set; }

    DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
}

