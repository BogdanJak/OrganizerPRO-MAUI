namespace OrganizerPRO.Application.Common.Interfaces;


public interface ISyncLocalizationContext
{
    DbSet<Localizer> Localizers { get; set; }

    DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
}

