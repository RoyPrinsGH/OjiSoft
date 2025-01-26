using OjiSoftPortal;
using OjiSoftPortal.Services;

var portal = AppSetup.InitializeOjiSoftPortal(args);

try
{
    using (IServiceScope scope = portal.Services.CreateScope())
    {
        DatabaseSeedingService dbSeedingService = scope.ServiceProvider.GetRequiredService<DatabaseSeedingService>();

        await dbSeedingService.EnsureRolesInDatabase();
        await dbSeedingService.EnsureSystemUserExists();
        await dbSeedingService.EnsurePreRegisteredApplications();
    }

    portal.Logger.LogInformation("Startup complete");

    portal.Run();
}
catch (Exception ex)
{
    portal.Logger.LogCritical(ex, "Unhandled exception occurred! Closing app.");
    await portal.StopAsync();
    return;
}