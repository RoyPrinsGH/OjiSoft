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
public class AuthenticationController : Controller
{
    private readonly SignInManager<OjiUser> _signInManager;

    public AuthenticationController(SignInManager<OjiUser> signInManager)
    {
        _signInManager = signInManager;
    }

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
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Username)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            Console.WriteLine("User logged in. Redirecting to: " + model.ReturnURL);
            return Redirect(model.ReturnURL);
        }

        return View(model);
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();

        return RedirectToAction("/");
    }

    [HttpGet("accessdenied")]
    public IActionResult AccessDenied()
    {
        // just return some plain text
        return Content("Access Denied");
    }
}