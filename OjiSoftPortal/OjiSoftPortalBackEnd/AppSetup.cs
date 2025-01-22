using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OjiSoftPortal.Configuration;
using OjiSoftPortal.Data;
using OjiSoftPortal.Data.Models;
using OjiSoftPortal.Services;
using OpenIddict.Abstractions;
using System.Net;
using System.Threading.RateLimiting;

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

            services.Configure<Configuration.RateLimiterOptions>(builder.Configuration.GetSection(Configuration.RateLimiterOptions.RateLimitersSectionName));
            services.AddSingleton<IValidateOptions<Configuration.RateLimiterOptions>, RateLimiterOptionsValidator>();

            services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.JwtSectionName));
            services.AddSingleton<IValidateOptions<JwtOptions>, JwtOptionsValidator>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                    {
                        options.LoginPath = "/user/login";
                        options.LogoutPath = "/user/logout";
                    });

            services.AddLogging(
                options =>
                {
                    options.ClearProviders();
                    options.AddConsole();
                    // use trace level for logging during development
                    options.SetMinimumLevel(LogLevel.Trace);
                }
            );

            services.AddDbContext<OjiSoftDataContext>(
                options =>
                {
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
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
        }
    }
}
