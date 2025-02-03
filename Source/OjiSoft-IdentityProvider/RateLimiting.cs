using OjiSoft.IdentityProvider.Logging;
using System.Threading.RateLimiting;

namespace OjiSoft.IdentityProvider;

public class RateLimiting
{
    public const int RateLimitRejectionStatusCode = 429;

    public static void SetupRateLimiterServices(WebApplicationBuilder builder)
    {
        ILogger logger = new FileLogger("OjiSoft.IdentityProvider.RateLimiting");

        IConfigurationSection rateLimiters = builder.Configuration.GetSection("RateLimiters");

        if (rateLimiters.Exists() is false)
        {
            throw new ArgumentException("Rate limiters configuration section is missing.");
        }

        SetupConfigVersioningListener(rateLimiters);

        foreach (IConfigurationSection rateLimiter in rateLimiters.GetChildren())
        {
            (int limit, int period) = GetRateLimiterConfigValues(rateLimiter);

            logger.LogInformation("Adding rate limiter \"{rateLimiterName}\" with limit {limit} every {period} seconds", rateLimiter.Key, limit, period);

            if (rateLimiter.Key == ConfigKeys.GeneralRateLimiterName)
            {
                builder.Services.AddRateLimiter(
                    options =>
                    {
                        options.RejectionStatusCode = RateLimitRejectionStatusCode;
                        options.GlobalLimiter = PartitionedRateLimiter.Create(GetIPKeyedTokenBucketRateLimitPartitionBuilder(rateLimiter));
                    }
                );

                continue;
            }

            builder.Services.AddRateLimiter(
                options =>
                {
                    options.RejectionStatusCode = RateLimitRejectionStatusCode;
                    options.AddPolicy(rateLimiter.Key, GetIPKeyedTokenBucketRateLimitPartitionBuilder(rateLimiter));
                }
            );
        }
    }

    private static void SetupConfigVersioningListener(IConfigurationSection rateLimiters)
    {
        using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder
            .SetMinimumLevel(LogLevel.Trace)
            .AddConsole());

        ILogger logger = loggerFactory.CreateLogger<RateLimiting>();

        // ASP.NET Core has a bug where it calls the reload token callback twice when the configuration changes, after the first callback.
        // This flag is used to prevent the callback from being called twice.
        // We start at true so that the first callback is not skipped.
        bool doCallback = true;

        void registerRefresh()
        {
            rateLimiters.GetReloadToken().RegisterChangeCallback(
                _ =>
                {
                    registerRefresh();

                    doCallback = !doCallback;

                    if (doCallback is false)
                    {
                        return;
                    }

                    RateLimitConfigVersion++;

                    logger.LogWarning(
                        "Rate limiter configuration changed to version {RateLimitConfigVersion}. Current configuration: \n{CurrentConfig}", 
                        RateLimitConfigVersion, 
                        string.Join('\n', rateLimiters.GetChildren().Select(c => $"\t{c.Key}: limit = {c["Limit"]}, period = {c["Period"]}"))
                    );
                },
                null
            );
        }

        registerRefresh();
    }

    private static (int limit, int period) GetRateLimiterConfigValues(IConfigurationSection rateLimiter)
    {
        string? limitConfigString = rateLimiter["Limit"];
        string? periodConfigString = rateLimiter["Period"];

        if (limitConfigString is null || periodConfigString is null)
        {
            throw new ArgumentException("Rate limiter \"{rateLimiterName}\" is missing required configuration values.", rateLimiter.Key);
        }

        if (int.TryParse(limitConfigString, out int limit) is false)
        {
            throw new ArgumentException($"Rate limiter \"{rateLimiter.Key}\" has an invalid limit value \"{limitConfigString}\".", rateLimiter.Key);
        }

        if (int.TryParse(periodConfigString, out int period) is false)
        {
            throw new ArgumentException($"Rate limiter \"{rateLimiter.Key}\" has an invalid period value \"{periodConfigString}\".", rateLimiter.Key);
        }

        return (limit, period);
    }

    // This is a hack to force the rate limiter to reload its configuration,
    // since ASP.NET Core doesn't provide a way to clear the rate limiter cache.
    // chars are atomic, so this is thread-safe to increment.
    private static char RateLimitConfigVersion = '1';

    private static Func<HttpContext, RateLimitPartition<string>> GetIPKeyedTokenBucketRateLimitPartitionBuilder(IConfigurationSection rateLimiter)
    {
        return context =>
        {
            // Capture changes to the rate limiter configuration values
            (int limit, int period) = GetRateLimiterConfigValues(rateLimiter);
            return RateLimitPartition.GetTokenBucketLimiter(context.Connection.RemoteIpAddress?.ToString() + RateLimitConfigVersion, _ => BuildTokenBucketRateLimiterOptions(limit, period));
        };
    }

    private static TokenBucketRateLimiterOptions BuildTokenBucketRateLimiterOptions(int limit, int period)
    {
        return new TokenBucketRateLimiterOptions
        {
            TokenLimit = limit,
            TokensPerPeriod = limit,
            ReplenishmentPeriod = TimeSpan.FromSeconds(period),
            QueueLimit = 0,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
        };
    }
}