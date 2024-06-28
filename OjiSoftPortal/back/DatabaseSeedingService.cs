using Microsoft.AspNetCore.Identity;
using OjiSoftPortal.Data;
using OjiSoftPortal.Data.Models;

namespace OjiSoftPortal
{
    public class DatabaseSeedingService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<OjiUser> _userManager;
        private readonly ILogger<DatabaseSeedingService> _logger;
        private readonly IConfiguration _config;

        public DatabaseSeedingService(RoleManager<IdentityRole> roleManager, UserManager<OjiUser> userManager, ILogger<DatabaseSeedingService> logger, IConfiguration config)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
            _config = config;
        }

        public class RoleCreationException(string message) : Exception(message);

        public async Task EnsureRolesInDatabase()
        {
            foreach (var roleName in Roles.All)
            {
                if (await _roleManager.RoleExistsAsync(roleName))
                {
                    _logger.LogInformation("Role {roleName} already exists", roleName);
                    continue;
                }

                IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(roleName));

                if (result.Succeeded)
                {
                    _logger.LogInformation("Role {roleName} created successfully", roleName);
                }
                else
                {
                    _logger.LogError("Failed to create role {roleName}", roleName);
                    throw new RoleCreationException($"Failed to create role {roleName}");
                }
            }
        }

        public class PowerUserCreationException(string message) : Exception(message);

        public async Task EnsurePowerUserExists()
        {
            string powerUserName = _config["PowerUser:Name"] ?? throw new PowerUserCreationException("Power user name not found in configuration");

            if (await _userManager.FindByNameAsync(powerUserName) is null)
            {
                string powerUserPassword = _config["PowerUser:Password"] ?? throw new PowerUserCreationException("Power user password not found in configuration");

                OjiUser powerUser = new()
                {
                    UserName = powerUserName,
                };

                IdentityResult result = await _userManager.CreateAsync(powerUser, powerUserPassword);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Power user created successfully");
                    await _userManager.AddToRoleAsync(powerUser, "Admin");
                }
                else
                {
                    _logger.LogError("Failed to create power user");
                    throw new PowerUserCreationException("Failed to create power user");
                }
            }

            _logger.LogInformation("Power user already exists");
        }
    }
}
