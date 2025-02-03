using Microsoft.Extensions.Options;

namespace OjiSoft.IdentityProvider.Configuration;

public class RateLimiterOptionsValidator : IValidateOptions<RateLimiterOptions>
{
    public ValidateOptionsResult Validate(string? name, RateLimiterOptions options)
    {
        if (options.General.Limit <= 0)
        {
            return ValidateOptionsResult.Fail("General.Limit must be greater than 0.");
        }

        if (options.General.Period <= 0)
        {
            return ValidateOptionsResult.Fail("General.Period must be greater than 0.");
        }

        if (options.AnonymousResourceModification.Limit <= 0)
        {
            return ValidateOptionsResult.Fail("AnonymousResourceModification.Limit must be greater than 0.");
        }

        if (options.AnonymousResourceModification.Period <= 0)
        {
            return ValidateOptionsResult.Fail("AnonymousResourceModification.Period must be greater than 0.");
        }

        if (options.AuthenticatedResourceModification.Limit <= 0)
        {
            return ValidateOptionsResult.Fail("AuthenticatedResourceModification.Limit must be greater than 0.");
        }

        if (options.AuthenticatedResourceModification.Period <= 0)
        {
            return ValidateOptionsResult.Fail("AuthenticatedResourceModification.Period must be greater than 0.");
        }

        if (options.Authenticated.Limit <= 0)
        {
            return ValidateOptionsResult.Fail("Authenticated.Limit must be greater than 0.");
        }

        if (options.Authenticated.Period <= 0)
        {
            return ValidateOptionsResult.Fail("Authenticated.Period must be greater than 0.");
        }

        return ValidateOptionsResult.Success;
    }
}