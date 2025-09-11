using OrganizerPRO.Domain.Entities.Localizes;

namespace OrganizerPRO.Infrastructure.Persistence;


public class SyncLocalizationDbContext : DbContext, ISyncLocalizationDbContext, IDataProtectionKeyContext
{

    public SyncLocalizationDbContext(DbContextOptions<SyncLocalizationDbContext> options)
            : base(options)
    {
    }

    public virtual DbSet<Localizer> Localizers { get; set; }
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

}

