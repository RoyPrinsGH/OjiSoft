using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using OjiSoftPortal.Data.Models;

namespace OjiSoftPortal.Controllers
{
    public class RegisterUserRequest
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
    };

    public class UserProfileController : Controller
    {
        private readonly UserManager<OjiUser> _userManager;

        public UserProfileController(UserManager<OjiUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("/user/{id}")]
        public async Task<IResult> Index([FromRoute] string id)
        {
            if (ModelState.IsValid is false)
            {
                return Results.BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(
                new
                {
                    user.UserName,
                    user.Nickname,
                    user.NicknameColor,
                    user.ProfileMainColor,
                    user.ProfileSecondaryColor,
                    user.Level,
                }
            );
        }

        [HttpPost("/user")]
        [EnableRateLimiting(ConfigKeys.AnonymousResourceModificationRateLimiterName)]
        public async Task<IResult> Create([FromRoute] RegisterUserRequest request)
        {
            if (ModelState.IsValid is false)
            {
                return Results.BadRequest(ModelState);
            }

            var user = new OjiUser
            {
                UserName = request.UserName
            };

            IdentityResult result = await _userManager.CreateAsync(user, request.Password);

            return result.Succeeded ? Results.Created($"/user/{user.Id}", user) : Results.BadRequest(result.Errors);
        }

        [HttpPatch("/user/{id}")]
        [EnableRateLimiting(ConfigKeys.AnonymousResourceModificationRateLimiterName)]
        public async Task<IResult> Update([FromRoute] string id, [FromBody] RegisterUserRequest request)
        {
            if (ModelState.IsValid is false)
            {
                return Results.BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
            {
                return Results.NotFound();
            }

            user.UserName = request.UserName;

            IdentityResult result = await _userManager.UpdateAsync(user);

            return result.Succeeded ? Results.NoContent() : Results.BadRequest(result.Errors);
        }

        [HttpDelete("/user/{id}")]
        public async Task<IResult> Delete([FromRoute] string id)
        {
            if (ModelState.IsValid is false)
            {
                return Results.BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return Results.NotFound();
            }

            IdentityResult result = await _userManager.DeleteAsync(user);

            return result.Succeeded ? Results.NoContent() : Results.BadRequest(result.Errors);
        }
    }
}
