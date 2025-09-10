namespace OrganizerPRO.Application.Common.Extensions;

public static class SpecificationBuilderExtensions
{
    public static ISpecificationBuilder<T> Where<T>(
        this ISpecificationBuilder<T> builder,
        string keyword,
        bool condition = true,
        params Expression<Func<T, string?>>[] properties)
    {
        if (!condition || string.IsNullOrEmpty(keyword))
            return builder;

        if (properties == null || properties.Length == 0)
        {
            properties = typeof(T).GetProperties()
                .Where(p => p.PropertyType == typeof(string))
                .Select(p => CreatePropertyExpression<T>(p))
                .ToArray();
        }

        Expression? predicate = null;
        var parameter = Expression.Parameter(typeof(T), "x");

     
        foreach (var propertyExpression in properties)
        {
            if (propertyExpression.Body is not MemberExpression memberExpression)
                continue; 

            var propertyAccess = Expression.Property(parameter, (memberExpression.Member as PropertyInfo)!);

            var nullCheck = Expression.NotEqual(propertyAccess, Expression.Constant(null, typeof(string)));

            var containsMethod = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) });

            var containsCall = Expression.Call(propertyAccess, containsMethod!, Expression.Constant(keyword));

            var conditionExpression = Expression.AndAlso(nullCheck, containsCall);

            predicate = predicate == null ? conditionExpression : Expression.OrElse(predicate, conditionExpression);
        }


        if (predicate != null)
        {
            var lambda = Expression.Lambda<Func<T, bool>>(predicate, parameter);
            builder.Where(lambda);
        }

        return builder;
    }

    private static Expression<Func<T, string?>> CreatePropertyExpression<T>(PropertyInfo property)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var propertyAccess = Expression.Property(parameter, property);
        return Expression.Lambda<Func<T, string?>>(propertyAccess, parameter);
    }
}

