namespace OrganizerPRO.Application.Common.Interfaces.Identity;

public interface IUserContextAccessor
{
    UserContext? Current { get; }

    IDisposable Push(UserContext context);

    void Clear();
} 