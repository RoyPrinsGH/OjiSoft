using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace OjiSoftPortal.Configuration
{
    public class JwtOptions
    {
        public const string JwtSectionName = "Jwt";

        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpiryInMinutes { get; set; } = 60;

        public SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new(Encoding.UTF8.GetBytes(Key).Take(256 / 8).ToArray());
        }
    }
}
