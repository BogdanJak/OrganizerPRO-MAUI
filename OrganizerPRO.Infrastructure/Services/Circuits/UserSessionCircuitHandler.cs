namespace OrganizerPRO.Infrastructure.Services.Circuits;


public class UserSessionCircuitHandler : CircuitHandler
{

    private readonly IUsersStateContainer _usersStateContainer;
    private readonly AuthenticationStateProvider _authenticationStateProvider;


    public UserSessionCircuitHandler(
        IUsersStateContainer usersStateContainer,
        AuthenticationStateProvider authenticationStateProvider)
    {

        _usersStateContainer = usersStateContainer;
        _authenticationStateProvider = authenticationStateProvider;
    }


    public override async Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
        if (state.User.Identity?.IsAuthenticated == true)
        {
            var userId = state.User.GetUserId();
            if (!string.IsNullOrEmpty(userId))
            {
                _usersStateContainer.AddOrUpdate(circuit.Id, userId);
            }
        }

        await base.OnConnectionUpAsync(circuit, cancellationToken);
    }


    public override async Task OnConnectionDownAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        _usersStateContainer.Remove(circuit.Id);
        await base.OnConnectionDownAsync(circuit, cancellationToken);
    }


}
