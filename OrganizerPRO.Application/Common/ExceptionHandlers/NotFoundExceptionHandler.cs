namespace OrganizerPRO.Application.Common.ExceptionHandlers;


public sealed class NotFoundExceptionHandler<TRequest, TResponse, TException> : IRequestExceptionHandler<TRequest, TResponse, TException>
    where TRequest : IRequest<TResponse>
    where TResponse : IResult
    where TException : NotFoundException
{
    private readonly ILogger<NotFoundExceptionHandler<TRequest, TResponse, TException>> _logger;

    public NotFoundExceptionHandler(ILogger<NotFoundExceptionHandler<TRequest, TResponse, TException>> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }


    public Task Handle(TRequest request, TException exception, RequestExceptionHandlerState<TResponse> state,
        CancellationToken cancellationToken)
    {
         
            var failureResult = CreateFailureResult(exception.Message);
            state.SetHandled(failureResult);
            
            _logger.LogError(exception, 
                "NotFoundException occurred for request {RequestType}: {ErrorMessage}", 
                typeof(TRequest).Name, 
                exception.Message);
        
        
        return Task.CompletedTask;
    }


    private TResponse CreateFailureResult(string errorMessage)
    {
        if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            var resultType = typeof(TResponse).GetGenericArguments()[0];

            var failureMethod = typeof(Result<>)
                .MakeGenericType(resultType)
                .GetMethod("Failure", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string[]) }, null);

            var failureResultObj = failureMethod?.Invoke(null, new object[] { new[] { errorMessage } });

            return (TResponse)(failureResultObj ?? throw new ArgumentNullException(nameof(failureResultObj)));
        }
        else
        {
            return (TResponse)(object)Result.Failure(errorMessage);
        }
    }
}