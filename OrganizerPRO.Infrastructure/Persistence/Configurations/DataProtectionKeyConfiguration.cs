namespace OrganizerPRO.Infrastructure.Persistence.Configurations;


public class DataProtectionKeyConfiguration : IEntityTypeConfiguration<DataProtectionKey>
{
    public void Configure(EntityTypeBuilder<DataProtectionKey> builder)
    {
        builder.Property(x => x.Xml).HasMaxLength(4000);
    }
}
