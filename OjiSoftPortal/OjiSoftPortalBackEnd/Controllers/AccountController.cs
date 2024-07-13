using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OjiSoftPortal.Configuration;
using OjiSoftPortal.Data.Models;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;

namespace OjiSoftPortal.Controllers
{
    public class AccountController(UserManager<OjiUser> userManager) : Controller
    {
        [EnableRateLimiting(ConfigKeys.AnonymousResourceModificationRateLimiterName)]
        [HttpPost("/register")]
        public async Task<IResult> Register([FromBody] RegisterUserRequest request)
        {
            if (ModelState.IsValid is false)
            {
                return Results.BadRequest(ModelState);
            }

            var user = new OjiUser
            {
                UserName = request.UserName,
                Nickname = request.UserName,
                Level = 1,
            };

            var result = await userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                return Results.Ok();
            }
            else
            {
                return Results.BadRequest(result.Errors);
            }
        }
    }
}
