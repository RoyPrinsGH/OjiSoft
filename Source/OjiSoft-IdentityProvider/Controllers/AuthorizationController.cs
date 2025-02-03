using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OjiSoft.IdentityProvider.Data.Models;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace OjiSoft.IdentityProvider.Controllers;

public class AuthorizationController(UserManager<OjiUser> userManager) : Controller
{
    private readonly UserManager<OjiUser> _userManager = userManager;

    [HttpGet("~/connect/authorize")]
    [HttpPost("~/connect/authorize")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Authorize()
    {
        Console.WriteLine("Authorize called with query string: " + Request.QueryString.Value);
        var request = HttpContext.GetOpenIddictServerRequest() ??
            throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        // Retrieve the user principal stored in the authentication cookie.
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // If the user principal can't be extracted, redirect the user to the login page.
        if (!result.Succeeded)
        {
            Console.WriteLine("User not authenticated. Redirecting to login page.");
            return Challenge(
                authenticationSchemes: CookieAuthenticationDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties
                {
                    RedirectUri = Request.PathBase + Request.Path + QueryString.Create(
                        Request.HasFormContentType ? Request.Form.ToList() : Request.Query.ToList())
                });
        }

        // Get the database user
        var user = await _userManager.GetUserAsync(result.Principal) ??
            throw new InvalidOperationException("The user details cannot be retrieved.");

        // Create a new claims principal
        var claims = new List<Claim>
        {
            new Claim(OpenIddictConstants.Claims.Subject, user.Id.ToString())
                .SetDestinations(OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken),
            new Claim(OpenIddictConstants.Claims.Name, user.UserName ?? user.Id.ToString())
                .SetDestinations(OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken),
        };

        var roles = await _userManager.GetRolesAsync(user);

        foreach (var role in roles)
        {
            claims.Add(new Claim(OpenIddictConstants.Claims.Role, role)
                .SetDestinations(OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken));
        }

        var claimsIdentity = new ClaimsIdentity(claims, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        // Set requested scopes (this is not done automatically)
        claimsPrincipal.SetScopes(request.GetScopes());

        // Signing in with the OpenIddict authentiction scheme trigger OpenIddict to issue a code (which can be exchanged for an access token)
        return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    [HttpPost("~/connect/token"), Produces("application/json")]
    public async Task<IActionResult> Exchange()
    {
        Console.WriteLine("Exchange called");
        
        var request = HttpContext.GetOpenIddictServerRequest() ??
            throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        ClaimsPrincipal claimsPrincipal;

        if (request.IsAuthorizationCodeGrantType())
        {
            // Retrieve the claims principal stored in the authorization code
            claimsPrincipal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal!;
        }
        else if (request.IsRefreshTokenGrantType())
        {
            // Retrieve the claims principal stored in the refresh token
            claimsPrincipal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal!;
        }
        else
        {
            throw new InvalidOperationException("The specified grant type is not supported.");
        }

        return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    [HttpGet("~/connect/logout")]
    public async Task<IActionResult> Logout()
    {
        Console.WriteLine("Logout called");
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return SignOut(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
}