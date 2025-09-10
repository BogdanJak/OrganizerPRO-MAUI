namespace OrganizerPRO.Infrastructure.Persistence.Conversions;


public static class ValueConversionExtensions
{
    public static PropertyBuilder<T> HasJsonConversion<T>(this PropertyBuilder<T> propertyBuilder)
    {
        var options = DefaultJsonSerializerOptions.Options;

        var converter = new ValueConverter<T, string>(
            v => JsonSerializer.Serialize(v, options),
            v => string.IsNullOrEmpty(v) ? default : JsonSerializer.Deserialize<T>(v, options));

        var comparer = new ValueComparer<T>(
            (l, r) => JsonSerializer.Serialize(l, options) == JsonSerializer.Serialize(r, options),
            v => v == null ? 0 : JsonSerializer.Serialize(v, options).GetHashCode(),
            v => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(v, options), options));

        propertyBuilder.HasConversion(converter);
        propertyBuilder.Metadata.SetValueComparer(comparer);

        return propertyBuilder;
    }
}
