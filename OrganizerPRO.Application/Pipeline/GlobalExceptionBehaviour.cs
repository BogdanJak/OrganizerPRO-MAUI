namespace OrganizerPRO.Application.Pipeline;

public class GlobalExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IUserContextAccessor _userContextAccessor;
    private readonly ILogger<TRequest> _logger;

    public GlobalExceptionBehaviour(ILogger<TRequest> logger, IUserContextAccessor userContextAccessor)
    {
        _logger = logger;
        _userContextAccessor = userContextAccessor;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;
            var userName = _userContextAccessor.Current?.UserName;
            _logger.LogError(ex,
                "Request: {RequestName} by User: {UserName} failed. Error: {ErrorMessage}. Request Details: {@Request}",
                requestName,
                userName,
                ex.Message,
                request);
            throw;
        }
    }
}