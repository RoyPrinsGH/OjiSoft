using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OjiSoft.IdentityProvider.Controllers;

public class UsersController(ILogger<UsersController> logger) : Controller
{
    private readonly ILogger<UsersController> _logger = logger;
}