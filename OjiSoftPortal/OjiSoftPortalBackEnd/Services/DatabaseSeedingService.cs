using Microsoft.AspNetCore.Identity;
using OjiSoftPortal.Data;
using OjiSoftPortal.Data.Models;
using OpenIddict.Abstractions;

namespace OjiSoftPortal.Services
{
    public class DatabaseSeedingService(RoleManager<IdentityRole> roleManager, UserManager<OjiUser> userManager, ILogger<DatabaseSeedingService> logger, IConfiguration config, IOpenIddictApplicationManager appManager)
    {
        public class RoleCreationException(string message) : Exception(message);

        public async Task EnsureRolesInDatabase()
        {
            foreach (var roleName in OjiRoles.All)
            {
                if (await roleManager.RoleExistsAsync(roleName))
                {
                    logger.LogInformation("Role \"{roleName}\" already exists", roleName);
                    continue;
                }

                IdentityResult result = await roleManager.CreateAsync(new IdentityRole(roleName));

                if (result.Succeeded)
                {
                    logger.LogInformation("Role \"{roleName}\" created successfully", roleName);
                }
                else
                {
                    string errors = string.Join(", ", result.Errors.Select(e => e.Description));

                    logger.LogError("Failed to create role \"{roleName}\": {errors}", roleName, errors);
                    throw new RoleCreationException($"Failed to create role \"{roleName}\": {errors}");
                }
            }
        }

        public class PowerUserCreationException(string message) : Exception(message);

        public async Task EnsurePowerUserExists()
        {
            string powerUserName = config[ConfigKeys.PowerUserName] ?? throw new PowerUserCreationException("Power user name not found in configuration");

            if (await userManager.FindByNameAsync(powerUserName) is null)
            {
                string powerUserPassword = config[ConfigKeys.PowerUserPassword] ?? throw new PowerUserCreationException("Power user password not found in configuration");

                OjiUser powerUser = new()
                {
                    UserName = powerUserName,
                    Level = 999,
                };

                IdentityResult result = await userManager.CreateAsync(powerUser, powerUserPassword);

                if (result.Succeeded)
                {
                    logger.LogInformation("Power user \"{powerUserName}\" created successfully", powerUserName);
                    await userManager.AddToRoleAsync(powerUser, OjiRoles.Admin);
                    await userManager.AddToRoleAsync(powerUser, OjiRoles.Member);
                }
                else
                {
                    string errors = string.Join(", ", result.Errors.Select(e => e.Description));

                    logger.LogError("Failed to create power user \"{powerUserName}\": {errors}", powerUserName, errors);
                    throw new PowerUserCreationException($"Failed to create power user \"{powerUserName}\": {errors}");
                }
            }

            logger.LogInformation("Power user \"{powerUserName}\" already exists", powerUserName);
        }

        class OjiAppCreationException(string message) : Exception(message);

        class OjiApplication
        {
            public required string Name { get; set; }
            public required string Description { get; set; }
            public required string PostLogoutRedirectUri { get; set; }
            public required string RedirectUri { get; set; }
            public required string ClientId { get; set; }
        }

        public async Task EnsurePreRegisteredApplications()
        {
            var ojiApps = config.GetSection(ConfigKeys.PreRegisteredApplications).GetChildren().Select(c => c.Get<OjiApplication>()!);

            foreach (OjiApplication ojiApp in ojiApps)
            {
                var descriptor = new OpenIddictApplicationDescriptor
                {
                    ClientId = ojiApp.ClientId,
                    ConsentType = OpenIddictConstants.ConsentTypes.Explicit,
                    ClientType = OpenIddictConstants.ClientTypes.Public,
                    DisplayName = ojiApp.Name,
                    PostLogoutRedirectUris = { new Uri(ojiApp.PostLogoutRedirectUri) },
                    RedirectUris = { new Uri(ojiApp.RedirectUri) },
                    Permissions =
                    {
                        OpenIddictConstants.Permissions.Endpoints.Authorization,
                        OpenIddictConstants.Permissions.Endpoints.Token,
                        OpenIddictConstants.Permissions.Endpoints.Logout,
                        OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                        OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                        OpenIddictConstants.Permissions.ResponseTypes.Code,
                        OpenIddictConstants.Permissions.Scopes.Email,
                        OpenIddictConstants.Permissions.Scopes.Profile,
                        OpenIddictConstants.Permissions.Scopes.Roles,
                    },
                    Requirements =
                    {
                        OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange
                    }
                };

                if (await appManager.FindByClientIdAsync(ojiApp.ClientId) is object client)
                {
                    await appManager.UpdateAsync(client, descriptor);
                    logger.LogInformation("Application \"{ojiApp.Name}\" updated", ojiApp.Name);

                    continue;
                }

                await appManager.CreateAsync(descriptor);
                logger.LogInformation("Application \"{ojiApp.Name}\" created successfully", ojiApp.Name);
            }
        }
    }
}
