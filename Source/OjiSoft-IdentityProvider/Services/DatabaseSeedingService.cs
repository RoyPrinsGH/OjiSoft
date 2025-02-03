using System.Reflection;
using Microsoft.AspNetCore.Identity;
using OjiSoft.IdentityProvider.Data;
using OjiSoft.IdentityProvider.Data.Models;
using OpenIddict.Abstractions;

namespace OjiSoft.IdentityProvider.Services;

public class DatabaseSeedingService(RoleManager<IdentityRole> roleManager, UserManager<OjiUser> userManager, ILogger<DatabaseSeedingService> logger, IConfiguration config, IOpenIddictApplicationManager appManager)
{
    public class RoleCreationException(string message) : Exception(message);

    public async Task EnsureRolesInDatabase()
    {
        // Get all roles using reflection

        Type rolesType = typeof(OjiRoles);
        FieldInfo[] roleFields = rolesType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
        var roles = roleFields.Select(f => f.GetValue(null) as string).Where(r => r is not null).Select(r => r!);

        foreach (var roleName in roles)
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

    public class SystemUserCreationException(string message) : Exception(message);

    public async Task EnsureSystemUserExists()
    {
        // Remove all system accounts before creating a new one
        var systemAccs = await userManager.GetUsersInRoleAsync(OjiRoles.System);
        foreach (OjiUser systemAcc in systemAccs)
        {
            await userManager.DeleteAsync(systemAcc);

            logger.LogWarning("Existing system account \"{systemAcc.UserName}\" deleted", systemAcc.UserName);
        }

        string systemUserName = config[ConfigKeys.SystemAccountUserName] ?? throw new SystemUserCreationException("Power user name not found in configuration");
        string systemUserPassword = config[ConfigKeys.SystemAccountPassword] ?? throw new SystemUserCreationException("Power user password not found in configuration");

        // We cannot create a system user if a non-system user with the same name already exists
        if (await userManager.FindByNameAsync(systemUserName) is OjiUser existingUserWithSystemUsername)
        {
            throw new SystemUserCreationException($"A non-system user with the name \"{systemUserName}\" already exists");
        }

        OjiUser systemUser = new()
        {
            UserName = systemUserName,
            Level = 999,
        };

        IdentityResult result = await userManager.CreateAsync(systemUser, systemUserPassword);

        if (result.Succeeded)
        {
            logger.LogInformation("System user \"{systemUserName}\" created successfully", systemUserName);
            await userManager.AddToRoleAsync(systemUser, OjiRoles.System);
            await userManager.AddToRoleAsync(systemUser, OjiRoles.Admin);
            await userManager.AddToRoleAsync(systemUser, OjiRoles.Member);
        }
        else
        {
            string errors = string.Join(", ", result.Errors.Select(e => e.Description));

            logger.LogError("Failed to create system user \"{systemUserName}\": {errors}", systemUserName, errors);
            throw new SystemUserCreationException($"Failed to create system user \"{systemUserName}\": {errors}");
        }
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