namespace OrganizerPRO.Infrastructure.Services.Identity;


public class UserContextHubFilter : IHubFilter
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IUserContextAccessor _userContextAccessor;

    private const string Key = "__user_ctx";

    public UserContextHubFilter(IServiceScopeFactory scopeFactory, IUserContextAccessor userContextAccessor)
    {
        _scopeFactory = scopeFactory;
        _userContextAccessor = userContextAccessor;
    }

    public async ValueTask<object?> InvokeMethodAsync(HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object?>> next)
    {
        invocationContext.Context.Items.TryGetValue(Key, out var val);
        var user = val as UserContext;

        if (user != null)
        {
            using (_userContextAccessor.Push(user))
            {
                return await next(invocationContext);
            }
        }
        else
        {
            return await next(invocationContext);
        }
    }


    public async Task OnConnectedAsync(HubLifetimeContext context, Func<HubLifetimeContext, Task> next)
    {
        var principal = context.Context.User;
        if (principal?.Identity?.IsAuthenticated == true)
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var loader = scope.ServiceProvider.GetRequiredService<IUserContextLoader>();
            var userContext = await loader.LoadAsync(principal, context.Context.ConnectionAborted);
            context.Context.Items[Key] = userContext;
        }

        await next(context);
    }


    public async Task OnDisconnectedAsync(HubLifetimeContext context, Exception? exception, Func<HubLifetimeContext, Exception?, Task> next)
    {
        _userContextAccessor.Clear();

        await next(context, exception);
    }
}
