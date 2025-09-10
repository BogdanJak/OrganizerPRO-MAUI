namespace OrganizerPRO.Infrastructure.Services.Identity;


public class UserProfileState : IUserProfileState, IDisposable
{
    private TimeSpan RefreshInterval => TimeSpan.FromSeconds(60);
    private UserProfile _currentValue = UserProfile.Empty;
    private string? _currentUserId;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly IMapper _mapper;
    private readonly IFusionCache _fusionCache;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<UserProfileState> _logger;

    public UserProfileState(
        IMapper mapper,
        IServiceScopeFactory scopeFactory,
        IFusionCache fusionCache,
        ILogger<UserProfileState> logger)
    {
        _scopeFactory = scopeFactory;
        _mapper = mapper;
        _fusionCache = fusionCache;
        _logger = logger;
    }

    public UserProfile Value => _currentValue;
    public event EventHandler<UserProfile>? Changed;


    public async Task EnsureInitializedAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return;

        if (_currentUserId == userId && _currentValue != UserProfile.Empty)
            return;

        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            if (_currentUserId == userId && _currentValue != UserProfile.Empty)
                return;

            var result = await LoadUserProfileFromDatabaseAsync(userId, cancellationToken);

            if (result is not null)
            {
                var newProfile = result.ToUserProfile();
                _currentUserId = userId;
                SetInternal(newProfile);
            }
            else
            {
                _currentUserId = userId;
                SetInternal(UserProfile.Empty with { UserId = userId });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize user profile for {UserId}", userId);
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }


    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_currentUserId))
            return;

        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            var cacheKey = UserCacheKeys.GetCacheKey(_currentUserId, UserCacheType.Application);
            await _fusionCache.RemoveAsync(cacheKey);

            var result = await LoadUserProfileFromDatabaseAsync(_currentUserId, cancellationToken);

            if (result is not null)
            {
                var newProfile = result.ToUserProfile();
                SetInternal(newProfile);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh user profile for {UserId}", _currentUserId);
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }


    public void Set(UserProfile userProfile)
    {
        ArgumentNullException.ThrowIfNull(userProfile);
        _currentUserId = userProfile.UserId;
        SetInternal(userProfile);
        _ = Task.Run(async () =>
        {
            var cacheKey = UserCacheKeys.GetCacheKey(userProfile.UserId, UserCacheType.Application);
            await _fusionCache.RemoveAsync(cacheKey);
        });
    }


    public void UpdateLocal(
        string? profilePictureDataUrl = null,
        string? displayName = null,
        string? phoneNumber = null,
        string? timeZoneId = null,
        string? languageCode = null)
    {
        if (_currentValue == UserProfile.Empty)
        {
            return;
        }

        var updatedProfile = _currentValue with
        {
            ProfilePictureDataUrl = profilePictureDataUrl ?? _currentValue.ProfilePictureDataUrl,
            DisplayName = displayName ?? _currentValue.DisplayName,
            PhoneNumber = phoneNumber ?? _currentValue.PhoneNumber,
            TimeZoneId = timeZoneId ?? _currentValue.TimeZoneId,
            LanguageCode = languageCode ?? _currentValue.LanguageCode
        };

        SetInternal(updatedProfile);
        _ = Task.Run(async () =>
        {
            var cacheKey = UserCacheKeys.GetCacheKey(_currentValue.UserId, UserCacheType.Application);
            await _fusionCache.RemoveAsync(cacheKey);
        });
    }


    public void ClearCache()
    {
        if (!string.IsNullOrWhiteSpace(_currentUserId))
        {

            _ = Task.Run(async () =>
            {
                var cacheKey = UserCacheKeys.GetCacheKey(_currentUserId, UserCacheType.Application);
                await _fusionCache.RemoveAsync(cacheKey);
            });
        }
    }

    private void SetInternal(UserProfile newProfile)
    {
        var oldProfile = _currentValue;
        _currentValue = newProfile;

        if (!ReferenceEquals(oldProfile, newProfile))
        {
            Changed?.Invoke(this, newProfile);
        }
    }

    private string GetApplicationUserCacheKey(string userId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        return UserCacheKeys.GetCacheKey(userId, UserCacheType.Application);
    }


    private async Task<ApplicationUserDto?> LoadUserProfileFromDatabaseAsync(string userId, CancellationToken cancellationToken = default)
    {
        var key = GetApplicationUserCacheKey(userId);
        return await _fusionCache.GetOrSetAsync(
            key,
            async _ =>
            {
                using var scope = _scopeFactory.CreateScope();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                return await userManager.Users
                            .Where(x => x.Id == userId)
                            .Include(x => x.UserRoles).ThenInclude(x => x.Role)
                            .ProjectTo<ApplicationUserDto>(_mapper.ConfigurationProvider)
                            .FirstOrDefaultAsync(cancellationToken);
            },
            RefreshInterval);
    }


    public async Task SetLanguageAsync(string languageCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(languageCode) || string.IsNullOrWhiteSpace(_currentUserId))
            return;

        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByIdAsync(_currentUserId);
            if (user is null)
                return;

            user.LanguageCode = languageCode;
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Failed to update language. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            UpdateLocal(languageCode: languageCode);
            var cacheKey = UserCacheKeys.GetCacheKey(_currentUserId, UserCacheType.Application);
            await _fusionCache.RemoveAsync(cacheKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set language for {UserId}", _currentUserId);
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void Dispose()
    {
        _semaphore?.Dispose();
        GC.SuppressFinalize(this);
    }
}

