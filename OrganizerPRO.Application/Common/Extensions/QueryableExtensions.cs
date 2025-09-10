namespace OrganizerPRO.Application.Common.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> ApplySpecification<T>(this IQueryable<T> query, ISpecification<T> spec,
        bool evaluateCriteriaOnly = false) where T : class, IEntity
    {
        return SpecificationEvaluator.Default.GetQuery(query, spec, evaluateCriteriaOnly);
    }

    /// <returns>The paginated and projected data</returns>
    public static async Task<PaginatedData<TResult>> ProjectToPaginatedDataAsync<T, TResult>(
        this IOrderedQueryable<T> query, ISpecification<T> spec, int pageNumber, int pageSize,
        AutoMapper.IConfigurationProvider configuration, CancellationToken cancellationToken = default) where T : class, IEntity
    {
        var specificationEvaluator = SpecificationEvaluator.Default;
        var count = await specificationEvaluator.GetQuery(query.AsNoTracking(), spec).CountAsync();
        var data = await specificationEvaluator.GetQuery(query.AsNoTracking(), spec).Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ProjectTo<TResult>(configuration)
            .ToListAsync(cancellationToken);
        return new PaginatedData<TResult>(data, count, pageNumber, pageSize);
    }

    public static async Task<PaginatedData<TResult>> ProjectToPaginatedDataAsync<T, TResult>(
        this IOrderedQueryable<T> query, ISpecification<T> spec, int pageNumber, int pageSize,
        Func<T, TResult> mapperFunc, CancellationToken cancellationToken = default) where T : class, IEntity
    {
        var specificationEvaluator = SpecificationEvaluator.Default;
        var queryWithSpec = specificationEvaluator.GetQuery(query.AsNoTracking(), spec);

        var count = await queryWithSpec.CountAsync(cancellationToken);
        var data = await queryWithSpec
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = data.Select(x => mapperFunc(x)).ToList();

        return new PaginatedData<TResult>(items, count, pageNumber, pageSize);
    }

    public static IQueryable<T> WhereContainsKeyword<T>(this IQueryable<T> source, string keyword)
    {
        if (string.IsNullOrEmpty(keyword))
            return source;

        var parameter = Expression.Parameter(typeof(T), "x");
        var properties = typeof(T).GetProperties()
            .Where(p => p.PropertyType == typeof(string));

        Expression? predicate = null;

        foreach (var property in properties)
        {
            var propertyAccess = Expression.Property(parameter, property);
            var nullCheck = Expression.NotEqual(propertyAccess, Expression.Constant(null, typeof(string)));
            var containsMethod = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) });
            var containsCall = Expression.Call(propertyAccess, containsMethod!, Expression.Constant(keyword));

            var condition = Expression.AndAlso(nullCheck, containsCall);

            predicate = predicate == null ? condition : Expression.OrElse(predicate, condition);
        }

        if (predicate == null)
            return source;

        var lambda = Expression.Lambda<Func<T, bool>>(predicate, parameter);
        return source.Where(lambda);
    }



    #region OrderBy
    public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string orderByProperty)
    {
        var parts = orderByProperty.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        string property = parts[0];
        string direction = parts.Length > 1 && parts[1].Equals("Descending", StringComparison.OrdinalIgnoreCase)
            ? "OrderByDescending"
            : "OrderBy";

        return ApplyOrder(source, property, direction);
    }

    public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string orderByProperty)
    {
        return ApplyOrder(source, orderByProperty, "OrderByDescending");
    }

    public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string orderByProperty)
    {
        return ApplyOrder(source, orderByProperty, "ThenBy");
    }

    public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string orderByProperty)
    {
        return ApplyOrder(source, orderByProperty, "ThenByDescending");
    }

    private static IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, string property, string methodName)
    {
        var type = typeof(T);
        var propertyInfo = type.GetProperty(property, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (propertyInfo == null)
        {
            throw new ArgumentException($"Property '{property}' does not exist on type '{type.Name}'.");
        }

        var parameter = Expression.Parameter(type, "x");
        var propertyAccess = Expression.MakeMemberAccess(parameter, propertyInfo);
        var orderByExpression = Expression.Lambda(propertyAccess, parameter);

        var resultExpression = Expression.Call(
            typeof(Queryable),
            methodName,
            new[] { type, propertyInfo.PropertyType },
            source.Expression,
            Expression.Quote(orderByExpression));

        return (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(resultExpression);
    }

    public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string orderBy, string sortDirection)
    {
        return sortDirection.Equals("Descending", StringComparison.OrdinalIgnoreCase)
            ? source.OrderByDescending(orderBy)
            : source.OrderBy(orderBy);
    }
    #endregion


}
