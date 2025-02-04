using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OjiSoft.IdentityProvider.Data.Models;
using OjiSoft.IdentityProvider.Models;
using OpenIddict.Abstractions;

namespace OjiSoft.IdentityProvider.Controllers;

[Route("user")]
public class AuthenticationController(SignInManager<OjiUser> signInManager, ILogger<AuthenticationController> logger) : Controller
{
    private readonly SignInManager<OjiUser> _signInManager = signInManager;

    private readonly ILogger<AuthenticationController> _logger = logger;

    [HttpGet("login")]
    public IActionResult Login(string? returnUrl = null)
    {
        _logger.LogInformation("Login called with query string: {queryString}", Request.QueryString.Value);
        ViewData["ReturnUrl"] = returnUrl;
        return View("Login");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginModel model)
    {
        ViewData["ReturnUrl"] = model.ReturnURL;

        _logger.LogInformation("Login called with username: {username} and password: {password}", model.Username, model.Password);
        
        if (ModelState.IsValid) 
        {
            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            var user = await _signInManager.UserManager.FindByNameAsync(model.Username)
                        ?? throw new InvalidOperationException("The user details cannot be retrieved.");

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName ?? "Unknown"),
                new(OpenIddictConstants.Claims.Subject, user.Id),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            _logger.LogInformation("User logged in. Redirecting to: {returnUrl}", model.ReturnURL);
            return Redirect(model.ReturnURL);
        }

        return View(model);
    }

    [HttpGet("accessdenied")]
    public IActionResult AccessDenied()
    {
        // just return some plain text
        return Content("Access Denied");
    }
}