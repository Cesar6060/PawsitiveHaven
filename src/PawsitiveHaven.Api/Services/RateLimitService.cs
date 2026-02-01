using Microsoft.Extensions.Caching.Memory;

namespace PawsitiveHaven.Api.Services;

public interface IRateLimitService
{
    RateLimitResult CheckRateLimit(int userId);
    void RecordRequest(int userId);
    void RecordViolation(int userId);
    bool IsUserBanned(int userId);
}

public class RateLimitService : IRateLimitService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<RateLimitService> _logger;

    // Rate limit configuration
    private const int RequestsPerMinute = 20;
    private const int RequestsPerHour = 100;
    private const int RequestsPerDay = 500;
    private const int MaxViolationsBeforeBan = 5;
    private static readonly TimeSpan ViolationWindow = TimeSpan.FromHours(1);
    private static readonly TimeSpan BanDuration = TimeSpan.FromHours(24);

    public RateLimitService(IMemoryCache cache, ILogger<RateLimitService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public RateLimitResult CheckRateLimit(int userId)
    {
        // Check if user is banned
        if (IsUserBanned(userId))
        {
            var banExpiry = _cache.Get<DateTimeOffset>($"ban:{userId}");
            var retryAfter = banExpiry - DateTimeOffset.UtcNow;

            _logger.LogWarning("Banned user {UserId} attempted to send message", userId);

            return RateLimitResult.Banned(retryAfter > TimeSpan.Zero ? retryAfter : TimeSpan.FromMinutes(1));
        }

        // Check per-minute limit
        var minuteKey = $"ratelimit:minute:{userId}";
        var minuteCount = _cache.GetOrCreate(minuteKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
            return 0;
        });

        if (minuteCount >= RequestsPerMinute)
        {
            _logger.LogWarning("User {UserId} exceeded per-minute rate limit", userId);
            return RateLimitResult.Limited(TimeSpan.FromMinutes(1), "Too many messages. Please wait a moment.");
        }

        // Check per-hour limit
        var hourKey = $"ratelimit:hour:{userId}";
        var hourCount = _cache.GetOrCreate(hourKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
            return 0;
        });

        if (hourCount >= RequestsPerHour)
        {
            _logger.LogWarning("User {UserId} exceeded per-hour rate limit", userId);
            return RateLimitResult.Limited(TimeSpan.FromHours(1), "Hourly message limit reached. Please try again later.");
        }

        // Check per-day limit
        var dayKey = $"ratelimit:day:{userId}";
        var dayCount = _cache.GetOrCreate(dayKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);
            return 0;
        });

        if (dayCount >= RequestsPerDay)
        {
            _logger.LogWarning("User {UserId} exceeded per-day rate limit", userId);
            return RateLimitResult.Limited(TimeSpan.FromDays(1), "Daily message limit reached. Please try again tomorrow.");
        }

        return RateLimitResult.Allowed();
    }

    public void RecordRequest(int userId)
    {
        // Increment counters
        IncrementCounter($"ratelimit:minute:{userId}", TimeSpan.FromMinutes(1));
        IncrementCounter($"ratelimit:hour:{userId}", TimeSpan.FromHours(1));
        IncrementCounter($"ratelimit:day:{userId}", TimeSpan.FromDays(1));
    }

    public void RecordViolation(int userId)
    {
        var violationKey = $"violations:{userId}";
        var count = IncrementCounter(violationKey, ViolationWindow);

        _logger.LogWarning("Security violation recorded for user {UserId}. Total: {Count}", userId, count);

        if (count >= MaxViolationsBeforeBan)
        {
            BanUser(userId);
        }
    }

    public bool IsUserBanned(int userId)
    {
        return _cache.TryGetValue($"ban:{userId}", out _);
    }

    private void BanUser(int userId)
    {
        var banKey = $"ban:{userId}";
        var banExpiry = DateTimeOffset.UtcNow.Add(BanDuration);

        _cache.Set(banKey, banExpiry, BanDuration);

        _logger.LogError(
            "User {UserId} has been banned for {Duration} due to repeated security violations",
            userId,
            BanDuration
        );
    }

    private int IncrementCounter(string key, TimeSpan duration)
    {
        var count = _cache.GetOrCreate(key, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = duration;
            return 0;
        });

        count++;
        _cache.Set(key, count, duration);

        return count;
    }
}

public class RateLimitResult
{
    public bool IsAllowed { get; }
    public bool IsBanned { get; }
    public TimeSpan? RetryAfter { get; }
    public string? Message { get; }

    private RateLimitResult(bool isAllowed, bool isBanned, TimeSpan? retryAfter, string? message)
    {
        IsAllowed = isAllowed;
        IsBanned = isBanned;
        RetryAfter = retryAfter;
        Message = message;
    }

    public static RateLimitResult Allowed()
        => new(true, false, null, null);

    public static RateLimitResult Limited(TimeSpan retryAfter, string message)
        => new(false, false, retryAfter, message);

    public static RateLimitResult Banned(TimeSpan retryAfter)
        => new(false, true, retryAfter, "Your account has been temporarily restricted due to unusual activity. Please try again later.");
}
