namespace OrganizerPRO.Infrastructure.Services;


public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.UtcNow;
}
