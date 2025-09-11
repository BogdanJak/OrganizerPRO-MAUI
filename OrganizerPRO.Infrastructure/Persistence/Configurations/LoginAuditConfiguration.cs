namespace OrganizerPRO.Infrastructure.Persistence.Configurations;


public class LoginAuditConfiguration : IEntityTypeConfiguration<LoginAudit>
{
    public void Configure(EntityTypeBuilder<LoginAudit> builder)
    {

        builder.Property(x => x.UserId).HasMaxLength(450).IsRequired();
        builder.Property(x => x.UserName).HasMaxLength(256).IsRequired();
        builder.Property(x => x.IpAddress).HasMaxLength(45);
        builder.Property(x => x.BrowserInfo).HasMaxLength(1000);
        builder.Property(x => x.Region).HasMaxLength(500);
        builder.Property(x => x.Provider).HasMaxLength(100);

        // Add index for frequently queried fields
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.LoginTimeUtc);
        builder.HasIndex(x => new { x.UserId, x.LoginTimeUtc });

        // Configure relationship with ApplicationUser
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
