using OrganizerPRO.Domain.Entities.Localizes;

namespace OrganizerPRO.Infrastructure.Persistence;


public class SyncLocalizationContext : DbContext, ISyncLocalizationContext, IDataProtectionKeyContext
{

    public SyncLocalizationContext(DbContextOptions<SyncLocalizationContext> options)
            : base(options)
    {
    }

    public virtual DbSet<Localizer> Localizers { get; set; }
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

}

