namespace OrganizerPRO.Application.Common.ExceptionHandlers;


public sealed class DbExceptionHandler<TRequest, TResponse, TException> :
    IRequestExceptionHandler<TRequest, TResponse, TException>
    where TRequest : IRequest<TResponse>
    where TResponse : IResult
    where TException : DbUpdateException
{
    private static readonly ConcurrentDictionary<Type, Func<string[], object>> FailureFactoryCache = new();

    private static readonly string[] ConstraintPrefixes = ["PK_", "FK_", "IX_", "UQ_", "UC_"];
    public Task Handle(
        TRequest request,
        TException exception,
        RequestExceptionHandlerState<TResponse> state,
        CancellationToken cancellationToken)
    {
        var errors = GetUserFriendlyErrors(exception);
        var failureResult = CreateFailureResult(errors);

        state.SetHandled(failureResult);
        return Task.CompletedTask;
    }

    private TResponse CreateFailureResult(string[] errors)
    {
        var factory = FailureFactoryCache.GetOrAdd(typeof(TResponse), CreateFailureFactory);
        return (TResponse)factory(errors);
    }

    private static Func<string[], object> CreateFailureFactory(Type responseType)
    {
        var errorsParam = Expression.Parameter(typeof(string[]), "errors");

        if (responseType.IsGenericType &&
            responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var valueType = responseType.GetGenericArguments()[0];
            var genericResultType = typeof(Result<>).MakeGenericType(valueType);

            var failureMethod = genericResultType.GetMethod(
                "Failure",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                types: [typeof(string[])],
                modifiers: null)
                ?? throw new InvalidOperationException(
                    $"Could not find the 'Failure' method on Result<{valueType.Name}>.");

            var call = Expression.Call(null, failureMethod, errorsParam);
            var cast = Expression.Convert(call, typeof(object));
            return Expression.Lambda<Func<string[], object>>(cast, errorsParam).Compile();
        }
        else
        {
            var failureMethod = typeof(Result).GetMethod(
                "Failure",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                types: [typeof(string[])],
                modifiers: null)
                ?? throw new InvalidOperationException("Could not find the 'Failure' method on Result.");

            var call = Expression.Call(null, failureMethod, errorsParam);
            var cast = Expression.Convert(call, typeof(object));
            return Expression.Lambda<Func<string[], object>>(cast, errorsParam).Compile();
        }
    }

    private string[] GetUserFriendlyErrors(DbUpdateException exception) =>
        exception switch
        {
            UniqueConstraintException uniqueEx => GetUniqueConstraintErrors(uniqueEx),
            CannotInsertNullException => ["Required fields are missing. Please fill in all mandatory information."],
            MaxLengthExceededException => ["Input data exceeds maximum length. Please reduce the size of your entries."],
            NumericOverflowException => ["Numeric values are outside the valid range. Please enter appropriate numbers."],
            ReferenceConstraintException referenceEx => GetReferenceConstraintErrors(referenceEx),
            _ => GetGenericDatabaseErrors(exception)
        };

    private static string[] GetUniqueConstraintErrors(UniqueConstraintException exception)
    {
        var tableName = GetTableName(exception.SchemaQualifiedTableName);
        var propertiesStr = GetConstraintProperties(exception.ConstraintProperties);

        var message = !string.IsNullOrWhiteSpace(propertiesStr) && propertiesStr != "specified properties"
            ? $"A record with the same {propertiesStr} already exists. Each {propertiesStr} must be unique."
            : "A duplicate record was found. Please ensure all values are unique.";

        return [message];
    }

    private static string[] GetReferenceConstraintErrors(ReferenceConstraintException exception)
    {
        return ["Cannot complete this operation because the record has dependent data. Please remove related records first."];
    }

    private string[] GetGenericDatabaseErrors(DbUpdateException exception)
    {
        return ["A database error occurred. Please try again or contact support if the issue persists."];
    }

    #region Helper Methods

    private static string GetTableName(string? schemaQualifiedTableName)
    {
        if (string.IsNullOrWhiteSpace(schemaQualifiedTableName))
            return "table";

        // Extract table name from schema.table format
        var parts = schemaQualifiedTableName.Split('.');
        var tableName = parts.Length > 1 ? parts[^1] : schemaQualifiedTableName;
        
        // Remove brackets and quotes if present
        tableName = tableName.Trim('[', ']', '"', '\'');
        
        return string.IsNullOrWhiteSpace(tableName) ? "table" : tableName;
    }

    private static string GetConstraintProperties(IReadOnlyList<string>? constraintProperties)
    {
        if (constraintProperties is null or { Count: 0 })
            return "specified properties";

        return constraintProperties.Count == 1 
            ? constraintProperties[0] 
            : string.Join(", ", constraintProperties);
    }


    private static string GetConstraintName(string? constraintName)
    {
        if (string.IsNullOrWhiteSpace(constraintName))
            return string.Empty;

        var clean = constraintName.AsSpan();
        foreach (var prefix in ConstraintPrefixes)
        {
            if (clean.StartsWith(prefix))
            {
                clean = clean[prefix.Length..];
                break;
            }
        }

        return clean.IsEmpty ? string.Empty : clean.ToString();
    }

    #endregion
}
