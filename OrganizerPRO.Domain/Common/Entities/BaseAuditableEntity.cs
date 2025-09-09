namespace OrganizerPRO.Domain.Common.Entities;
public abstract class BaseAuditableEntity : BaseEntity, IAuditableEntity
{
    public virtual DateTime? CreatedAt { get; set; }

    public virtual string? CreatedById { get; set; }

    public virtual DateTime? LastModifiedAt { get; set; }

    public virtual string? LastModifiedById { get; set; }

}

public interface IAuditableEntity
{
    DateTime? CreatedAt { get; set; }

    string? CreatedById { get; set; }

    DateTime? LastModifiedAt { get; set; }

    string? LastModifiedById { get; set; }

}

