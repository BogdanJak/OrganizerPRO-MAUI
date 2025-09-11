namespace OrganizerPRO.Infrastructure.Persistence.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            // Each User can have many UserRoles
            builder.HasMany(u => u.UserRoles)
                .WithOne(ur => ur.User) // powiązanie z nawigacją w ApplicationUserRole
                .HasForeignKey(ur => ur.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Each User can have many UserClaims
            builder.HasMany(u => u.UserClaims)
                .WithOne(uc => uc.User)
                .HasForeignKey(uc => uc.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Each User can have many UserLogins
            builder.HasMany(u => u.Logins)
                .WithOne(l => l.User)
                .HasForeignKey(l => l.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Each User can have many UserTokens
            builder.HasMany(u => u.Tokens)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Relacja z Tenant
            builder.HasOne(u => u.Tenant)
                .WithMany()
                .HasForeignKey(u => u.TenantId);

            builder.Navigation(u => u.Tenant).AutoInclude();
        }
    }

    public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            builder.HasIndex(x => x.NormalizedName)
                   .HasDatabaseName("RoleNameIndex")
                   .IsUnique(false);

            builder.HasIndex(x => new { x.TenantId, x.Name }).IsUnique();

            builder.HasOne(r => r.Tenant)
                   .WithMany()
                   .HasForeignKey(r => r.TenantId);

            builder.Navigation(r => r.Tenant).AutoInclude();
        }
    }

    public class ApplicationRoleClaimConfiguration : IEntityTypeConfiguration<ApplicationRoleClaim>
    {
        public void Configure(EntityTypeBuilder<ApplicationRoleClaim> builder)
        {
            builder.HasOne(rc => rc.Role)
                   .WithMany(r => r.RoleClaims)
                   .HasForeignKey(rc => rc.RoleId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class ApplicationUserRoleConfiguration : IEntityTypeConfiguration<ApplicationUserRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationUserRole> builder)
        {
            builder.HasOne(ur => ur.Role)
                   .WithMany(r => r.UserRoles)
                   .HasForeignKey(ur => ur.RoleId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ur => ur.User)
                   .WithMany(u => u.UserRoles)
                   .HasForeignKey(ur => ur.UserId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class ApplicationUserClaimConfiguration : IEntityTypeConfiguration<ApplicationUserClaim>
    {
        public void Configure(EntityTypeBuilder<ApplicationUserClaim> builder)
        {
            builder.HasOne(uc => uc.User)
                   .WithMany(u => u.UserClaims)
                   .HasForeignKey(uc => uc.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class ApplicationUserLoginConfiguration : IEntityTypeConfiguration<ApplicationUserLogin>
    {
        public void Configure(EntityTypeBuilder<ApplicationUserLogin> builder)
        {
            builder.HasKey(l => new { l.LoginProvider, l.ProviderKey });

            builder.HasOne(l => l.User)
                   .WithMany(u => u.Logins)
                   .HasForeignKey(l => l.UserId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class ApplicationUserTokenConfiguration : IEntityTypeConfiguration<ApplicationUserToken>
    {
        public void Configure(EntityTypeBuilder<ApplicationUserToken> builder)
        {
            builder.HasKey(t => new { t.UserId, t.LoginProvider, t.Name });

            builder.HasOne(t => t.User)
                   .WithMany(u => u.Tokens)
                   .HasForeignKey(t => t.UserId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
