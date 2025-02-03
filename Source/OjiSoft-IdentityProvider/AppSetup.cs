using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using OjiSoftPortal.Configuration;
using OjiSoftPortal.Data;
using OjiSoftPortal.Data.Models;
using OjiSoftPortal.Services;
using OpenIddict.Abstractions;

namespace OjiSoftPortal
{
    public static class AppSetup
    {
        /// <summary>
        /// Creates the app and sets up the application services, middleware and endpoints.
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns>The set up web app</returns>
        public static WebApplication InitializeOjiSoftPortal(params string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            SetupServices(builder);

            WebApplication app = builder.Build();

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors(
                options =>
                {
                    options.AllowAnyOrigin()
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                }
            );

            app.UseExceptionHandler("/error");
            app.UseRateLimiter();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles();

            MapMinimalAPIEndPoints(app);

            app.MapControllers();

            app.Lifetime.ApplicationStopping.Register(() => app.Logger.LogCritical("App is stopping gracefully..."));
            app.Lifetime.ApplicationStopped.Register(() => app.Logger.LogCritical("App has stopped"));

            return app;
        }

        public static void SetupServices(WebApplicationBuilder builder)
        {
            IServiceCollection services = builder.Services;

            RateLimiting.SetupRateLimiterServices(builder);

            services.Configure<RateLimiterOptions>(builder.Configuration.GetSection(RateLimiterOptions.RateLimitersSectionName));
            services.AddSingleton<IValidateOptions<RateLimiterOptions>, RateLimiterOptionsValidator>();

            services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.JwtSectionName));
            services.AddSingleton<IValidateOptions<JwtOptions>, JwtOptionsValidator>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                    {
                        options.LoginPath = "/user/login";
                        options.LogoutPath = "/user/logout";
                        options.AccessDeniedPath = "/user/accessdenied";
                    });

            services.AddLogging(
                options =>
                {
                    options.ClearProviders();
                    options.AddConsole();
                    // use trace level for logging during development
                    options.SetMinimumLevel(LogLevel.Trace);
                    options.AddFilter<ConsoleLoggerProvider>("Microsoft", LogLevel.Warning);
                }
            );

            services.AddDbContext<OjiSoftDataContext>(
                options =>
                {
                    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

                    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
                    {
                        Console.WriteLine("Production environment, using MySQL database");
                        options.UseMySql(connectionString, ServerVersion.Create(Version.Parse("8.0.41"), Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MySql));
                    }
                    else 
                    {
                        Console.WriteLine("Using SQL Server database");
                        options.UseSqlServer(connectionString);
                    }

                    options.UseOpenIddict();
                }
            );

            services.AddIdentity<OjiUser, IdentityRole>(
                options =>
                {
                    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                    options.Password.RequiredLength = 12;
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
                })
                .AddEntityFrameworkStores<OjiSoftDataContext>()
                .AddDefaultTokenProviders();

            // This way we get the validator to fire too
            JwtOptions jwtOptions = services.BuildServiceProvider().GetRequiredService<IOptions<JwtOptions>>().Value;

            services.AddOpenIddict()
                    .AddCore(
                        options =>
                        {
                            options.UseEntityFrameworkCore()
                                   .UseDbContext<OjiSoftDataContext>();
                        }
                    )
                    .AddServer(
                        options =>
                        {
                            options.SetTokenEndpointUris("connect/token")
                                   .SetAuthorizationEndpointUris("connect/authorize")
                                   .SetLogoutEndpointUris("connect/logout")
                                   .SetUserinfoEndpointUris("connect/userinfo")
                                   .SetIntrospectionEndpointUris("connect/introspect");

                            options.SetAccessTokenLifetime(TimeSpan.FromMinutes(jwtOptions.ExpiryInMinutes))
                                   .SetRefreshTokenLifetime(TimeSpan.FromDays(30));

                            options.RegisterScopes(OpenIddictConstants.Scopes.OfflineAccess, OpenIddictConstants.Scopes.Profile, OpenIddictConstants.Scopes.Roles);

                            options.AllowRefreshTokenFlow()
                                   .AllowAuthorizationCodeFlow()
                                   .RequireProofKeyForCodeExchange();

                            options.AddDevelopmentEncryptionCertificate()
                                   .AddDevelopmentSigningCertificate();

                            options.UseAspNetCore()
                                   .EnableTokenEndpointPassthrough()
                                   .EnableAuthorizationEndpointPassthrough()
                                   .EnableLogoutEndpointPassthrough()
                                   .DisableTransportSecurityRequirement();
                        }
                    )
                    .AddValidation(
                        options =>
                        {
                            options.SetIssuer(jwtOptions.Issuer);

                            options.UseLocalServer();

                            options.UseSystemNetHttp();
                            options.UseAspNetCore();
                        }
                    );

            services.AddControllersWithViews();

            services.AddAuthorization();

            services.AddScoped<DatabaseSeedingService>();
        }

        public static void MapMinimalAPIEndPoints(WebApplication app)
        {
            app.Map("/error", () => Results.BadRequest("An error occurred."));
            app.Map("/test", () => Results.Ok("Deployment 4"));
        }
    }
}
