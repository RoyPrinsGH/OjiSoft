using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OjiSoftPortal.Data.Models;
using OjiSoftPortal.Models;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace OjiSoftPortal.Controllers;

[Route("user")]
public class AuthenticationController(SignInManager<OjiUser> signInManager) : Controller
{
    private readonly SignInManager<OjiUser> _signInManager = signInManager;

    [HttpGet("login")]
    public IActionResult Login(string returnUrl = null)
    {
        Console.WriteLine("Login called with query string: " + Request.QueryString.Value);
        ViewData["ReturnUrl"] = returnUrl;
        return View("Login");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginModel model)
    {
        ViewData["ReturnUrl"] = model.ReturnURL;

        Console.WriteLine("Login called with username: " + model.Username + " and password: " + model.Password);
        
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
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(OpenIddictConstants.Claims.Subject, user.Id),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            Console.WriteLine("User logged in. Redirecting to: " + model.ReturnURL);
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