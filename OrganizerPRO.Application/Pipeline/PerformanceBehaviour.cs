namespace OrganizerPRO.Application.Pipeline;

public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IUserContextAccessor _userContextAccessor;
    private readonly ILogger<PerformanceBehaviour<TRequest, TResponse>> _logger;

    public PerformanceBehaviour(
        ILogger<PerformanceBehaviour<TRequest, TResponse>> logger,
        IUserContextAccessor userContextAccessor)
    {
        _logger = logger;
        _userContextAccessor = userContextAccessor;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var timer = Stopwatch.StartNew();

        Interlocked.Increment(ref RequestCounter.ExecutionCount);

        var response = await next().ConfigureAwait(false);

        timer.Stop();
        var elapsedMilliseconds = timer.ElapsedMilliseconds;

        var isStartupPhase = RequestCounter.ExecutionCount <= 50 || 
                            (DateTime.UtcNow - RequestCounter.StartTime).TotalSeconds < 60;
        
        var threshold = isStartupPhase ? 2000 : 500;

        if (elapsedMilliseconds > threshold)
        {
            var requestName = typeof(TRequest).Name;
            var userName = _userContextAccessor.Current?.UserName;
            var phase = isStartupPhase ? "Startup" : "Runtime";

            _logger.LogWarning(
                "Long-running request [{Phase}]: {RequestName} ({ElapsedMilliseconds}ms) {@Request} by {UserName}",
                phase, requestName, elapsedMilliseconds, request, userName);
        }

        return response;
    }
}

public static class RequestCounter
{
    public static int ExecutionCount;
    public static readonly DateTime StartTime = DateTime.UtcNow;
}