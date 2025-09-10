namespace OrganizerPRO.Domain.Common.Entities;


public interface ISoftDelete
{
    DateTime? DeletedAt { get; set; }
    string? DeletedById { get; set; }
}

