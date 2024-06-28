using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OjiSoftPortal;
using OjiSoftPortal.Data;
using OjiSoftPortal.Data.Models;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddLogging(options => options.AddConsole());

services.AddDbContext<OjiSoftDataContext>(
    options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
);

services.AddIdentity<OjiUser, IdentityRole>(
    options =>
    {
        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
        options.Password.RequiredLength = 12;
    })
    .AddEntityFrameworkStores<OjiSoftDataContext>()
    .AddDefaultTokenProviders();

services.AddScoped<DatabaseSeedingService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapPost("/register", async (RegisterUserRequest request, UserManager<OjiUser> userManager) =>
{
    var user = new OjiUser
    {
        UserName = request.UserName
    };

    var result = await userManager.CreateAsync(user, request.Password);

    if (result.Succeeded)
    {
        return Results.Created($"/user/{user.Id}", user);
    }
    else
    {
        return Results.BadRequest(result.Errors);
    }
});

app.MapGet("/user/{id}", async (string id, UserManager<OjiUser> userManager) =>
{
    var user = await userManager.FindByIdAsync(id);

    if (user == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(user);
});

app.MapPatch("/user/{id}", async (string id, RegisterUserRequest request, UserManager<OjiUser> userManager) =>
{
    var user = await userManager.FindByIdAsync(id);

    if (user == null)
    {
        return Results.NotFound();
    }

    user.UserName = request.UserName;

    var result = await userManager.UpdateAsync(user);

    if (result.Succeeded)
    {
        return Results.Ok(user);
    }
    else
    {
        return Results.BadRequest(result.Errors);
    }
});

app.MapDelete("/user/{id}", async (string id, UserManager<OjiUser> userManager) =>
{
    var user = await userManager.FindByIdAsync(id);

    if (user == null)
    {
        return Results.NotFound();
    }

    var result = await userManager.DeleteAsync(user);

    if (result.Succeeded)
    {
        return Results.NoContent();
    }
    else
    {
        return Results.BadRequest(result.Errors);
    }
});

app.UseHttpsRedirection();

try
{
    using (IServiceScope scope = app.Services.CreateScope())
    {
        DatabaseSeedingService dbSeedingService = scope.ServiceProvider.GetRequiredService<DatabaseSeedingService>();

        await dbSeedingService.EnsureRolesInDatabase();
        await dbSeedingService.EnsurePowerUserExists();
    }

    app.Logger.LogInformation("Startup complete");

    app.Lifetime.ApplicationStopping.Register(
        () =>
        {
            app.Logger.LogCritical("App is stopping gracefully...");
            Task.Delay(1000).Wait();
            app.Logger.LogCritical("App stopped");
        }
    );

    app.Run();
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "Unhandled exception occurred! Closing app.");
    await app.StopAsync();
    return;
}

class RegisterUserRequest
{
    public required string UserName { get; set; }
    public required string Password { get; set; }
};