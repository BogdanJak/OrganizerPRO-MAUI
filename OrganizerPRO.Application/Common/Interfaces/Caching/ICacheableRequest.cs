namespace OrganizerPRO.Application.Common.Interfaces.Caching;

public interface ICacheableRequest<TResponse> : IRequest<TResponse>
{
    string CacheKey => string.Empty;
    IEnumerable<string>? Tags { get; }
}
