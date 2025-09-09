namespace OrganizerPRO.Domain.Common.Entities;
public interface IMustHaveTenant
{
    Guid TenantId { get; set; }
}

public interface IMayHaveTenant
{
    Guid? TenantId { get; set; }
}

