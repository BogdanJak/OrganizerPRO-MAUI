namespace OrganizerPRO.Application.Common.Interfaces;


public interface IApplicationDbContext : IAsyncDisposable
{
    DbSet<Tenant> Tenants { get; set; }
    DbSet<Logger> Loggers { get; set; }
    DbSet<Localizer> Localizers { get; set; }
    DbSet<SystemLog> SystemLogs { get; set; }
    DbSet<AuditTrail> AuditTrails { get; set; }
    DbSet<LoginAudit> LoginAudits { get; set; }
    DbSet<UserLoginRiskSummary> UserLoginRiskSummaries { get; set; }
    DbSet<Permission> Permissions { get; set; }



    ChangeTracker ChangeTracker { get; }

    DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
