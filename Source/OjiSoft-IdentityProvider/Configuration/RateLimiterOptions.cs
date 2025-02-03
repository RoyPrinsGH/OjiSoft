namespace OjiSoft.IdentityProvider.Configuration;

public class RateLimiterConfiguration
{
    public string Name { get; set; } = string.Empty;
    public int Limit { get; set; } = 5;
    public int Period { get; set; } = 1;
}

public class RateLimiterOptions
{
    public const string RateLimitersSectionName = "RateLimiters";

    public RateLimiterConfiguration General { get; set; } = new();
    public RateLimiterConfiguration AnonymousResourceModification { get; set; } = new();
    public RateLimiterConfiguration AuthenticatedResourceModification { get; set; } = new();
    public RateLimiterConfiguration Authenticated { get; set; } = new();
}