using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OjiSoftPortal.Data.Models;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace OjiSoftPortal.Controllers
{
    public class AuthorizationController(IOpenIddictApplicationManager applicationManager, UserManager<OjiUser> userManager) : Controller
    {
        private readonly IOpenIddictApplicationManager _applicationManager = applicationManager;

        [HttpPost("~/connect/token"), Produces("application/json")]
        public async Task<IActionResult> Exchange()
        {
            var request = HttpContext.GetOpenIddictServerRequest();

            if (request is null)
            {
                throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");
            }

            if (request.IsClientCredentialsGrantType())
            {
                // Note: the client credentials are automatically validated by OpenIddict:
                // if client_id or client_secret are invalid, this action won't be invoked.

                var application = await _applicationManager.FindByClientIdAsync(request.ClientId!) ??
                    throw new InvalidOperationException("The application cannot be found.");

                // Create a new ClaimsIdentity containing the claims that
                // will be used to create an id_token, a token or a code.
                var identity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType, Claims.Name, Claims.Role);

                // Use the client_id as the subject identifier.
                identity.SetClaim(Claims.Subject, await _applicationManager.GetClientIdAsync(application));
                identity.SetClaim(Claims.Name, await _applicationManager.GetDisplayNameAsync(application));

                identity.SetDestinations(static claim => claim.Type switch
                {
                    // Allow the "name" claim to be stored in both the access and identity tokens
                    // when the "profile" scope was granted (by calling principal.SetScopes(...)).
                    Claims.Name when claim.Subject!.HasScope(Scopes.Profile)
                        => [Destinations.AccessToken, Destinations.IdentityToken],

                    // Otherwise, only store the claim in the access tokens.
                    _ => [Destinations.AccessToken]
                });

                identity.SetScopes(request.GetScopes());

                return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }
            else if (request.IsPasswordGrantType())
            {
                // Retrieve the user by using the user name
                var user = await userManager.FindByNameAsync(request.Username!);

                if (user is null)
                {
                    return LoginError();
                }

                // Ensure the user is allowed to sign in.
                if (await userManager.IsLockedOutAsync(user))
                {
                    return LoginError();
                }

                // Ensure the password matches
                if (!await userManager.CheckPasswordAsync(user, request.Password!))
                {
                    if (await userManager.GetLockoutEnabledAsync(user))
                    {
                        await userManager.AccessFailedAsync(user);

                        if (await userManager.IsLockedOutAsync(user))
                        {
                            return LoginError();
                        }
                    }

                    return LoginError();
                }

                // Reset the lockout count
                await userManager.ResetAccessFailedCountAsync(user);

                // Create a new ClaimsIdentity containing the claims that
                // will be used to create an id_token, a token or a code.
                var identity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType, Claims.Name, Claims.Role);

                identity.SetClaim(Claims.Subject, await userManager.GetUserIdAsync(user));
                identity.SetClaim(Claims.Name, await userManager.GetUserNameAsync(user));

                identity.SetDestinations(static claim => claim.Type switch
                {
                    Claims.Name => [Destinations.AccessToken, Destinations.IdentityToken],
                    _ => [Destinations.AccessToken]
                });

                identity.SetScopes(request.GetScopes());

                return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

                IActionResult LoginError()
                {
                    return BadRequest(new OpenIddictResponse
                    {
                        Error = Errors.InvalidGrant,
                        ErrorDescription = "The username or password is invalid."
                    });
                }
            }
            else if (request.IsRefreshTokenGrantType())
            {
                // Retrieve the claims principal stored in the refresh token.
                var principal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;

                // Retrieve the user profile corresponding to the refresh token.
                var user = await userManager.GetUserAsync(principal);
                if (user is null)
                {
                    return TokenError();
                }

                // Ensure the user is still allowed to sign in.
                if (await userManager.IsLockedOutAsync(user))
                {
                    return TokenError();
                }

                // Create a new ClaimsIdentity containing the claims that
                // will be used to create an id_token, a token or a code.
                var identity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType, Claims.Name, Claims.Role);

                identity.SetClaim(Claims.Subject, await userManager.GetUserIdAsync(user));
                identity.SetClaim(Claims.Name, await userManager.GetUserNameAsync(user));

                identity.SetDestinations(static claim => claim.Type switch
                {
                    Claims.Name => [Destinations.AccessToken, Destinations.IdentityToken],
                    _ => [Destinations.AccessToken]
                });

                identity.SetScopes(principal.GetScopes());

                return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

                IActionResult TokenError()
                {
                    return BadRequest(new OpenIddictResponse
                    {
                        Error = Errors.InvalidGrant,
                        ErrorDescription = "The refresh token is no longer valid."
                    });
                }
            }

            throw new NotImplementedException("The specified grant is not implemented.");
        }
    }
}
