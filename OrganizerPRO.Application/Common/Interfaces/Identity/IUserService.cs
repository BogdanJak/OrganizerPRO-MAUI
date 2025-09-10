namespace OrganizerPRO.Application.Common.Interfaces.Identity;

public interface IUserService
{
    List<ApplicationUserDto> DataSource { get; }
    event Func<Task>? OnChange;
    Task InitializeAsync();
    Task RefreshAsync();
}