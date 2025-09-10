namespace OrganizerPRO.Infrastructure.Persistence.Configurations;
public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        //builder.Ignore(e => e.DomainEvents);
    }
}
