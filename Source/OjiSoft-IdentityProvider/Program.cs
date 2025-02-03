using OjiSoft.IdentityProvider;
using OjiSoft.IdentityProvider.Data;
using OjiSoft.IdentityProvider.Services;
using Microsoft.EntityFrameworkCore;

var identityProvider = AppSetup.InitializeOjiSoftIdentityProvider(args);

try
{
    using (IServiceScope scope = identityProvider.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<OjiSoftDataContext>();

        identityProvider.Logger.LogInformation("Ensuring database is created and migrated");
        dbContext.Database.EnsureCreated();
        dbContext.Database.Migrate();

        identityProvider.Logger.LogInformation("Seeding database with roles, system user and pre-registered applications");
        DatabaseSeedingService dbSeedingService = scope.ServiceProvider.GetRequiredService<DatabaseSeedingService>();

        await dbSeedingService.EnsureRolesInDatabase();
        await dbSeedingService.EnsureSystemUserExists();
        await dbSeedingService.EnsurePreRegisteredApplications();
    }

    identityProvider.Logger.LogInformation("Startup complete");
    identityProvider.Run();
}
catch (Exception ex)
{
    identityProvider.Logger.LogCritical(ex, "Unhandled exception occurred! Closing app.");
    await identityProvider.StopAsync();
    return;
}