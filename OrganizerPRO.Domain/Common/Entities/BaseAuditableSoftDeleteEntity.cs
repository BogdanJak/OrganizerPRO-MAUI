namespace OrganizerPRO.Domain.Common.Entities;
public abstract class BaseAuditableSoftDeleteEntity : BaseAuditableEntity, ISoftDelete
{
    public DateTime? DeletedAt { get; set; }
    public string? DeletedById { get; set; }
}

