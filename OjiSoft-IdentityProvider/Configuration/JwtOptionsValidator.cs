using Microsoft.Extensions.Options;

namespace OjiSoftPortal.Configuration
{
    public class JwtOptionsValidator : IValidateOptions<JwtOptions>
    {
        public ValidateOptionsResult Validate(string? name, JwtOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.Key))
            {
                return ValidateOptionsResult.Fail("Key must be provided.");
            }

            if(options.Key.Length < 32)
            {
                return ValidateOptionsResult.Fail("Key must be at least 32 characters long.");
            }

            if (string.IsNullOrWhiteSpace(options.Issuer))
            {
                return ValidateOptionsResult.Fail("Issuer must be provided.");
            }

            if (string.IsNullOrWhiteSpace(options.Audience))
            {
                return ValidateOptionsResult.Fail("Audience must be provided.");
            }

            if (options.ExpiryInMinutes <= 0)
            {
                return ValidateOptionsResult.Fail("ExpiryInMinutes must be greater than 0.");
            }

            return ValidateOptionsResult.Success;
        }
    }
}
