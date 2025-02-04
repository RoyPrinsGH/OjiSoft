using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OjiSoft.IdentityProvider.Data;
using OpenIddict.Abstractions;
using OpenIddict.Core;

namespace OjiSoft.IdentityProvider.Controllers;

public class AppsController(IOpenIddictApplicationManager appManager, ILogger<AppsController> logger) : Controller
{
    private readonly IOpenIddictApplicationManager _appManager = appManager;
    private readonly ILogger<AppsController> _logger = logger;

    [Authorize(Roles = OjiRoles.System)]
    [HttpGet("apps/list")]
    public IActionResult ListApps()
    {
        _logger.LogWarning("User {user} requested list of apps", HttpContext.User?.Identity?.Name);

        var apps = _appManager.ListAsync();

        return Json(apps);
    }
}