using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IISBackend.API.Controllers;

[ApiController]
[Route("Account")]
public class AccountController : ControllerBase
{
    [HttpPost("Login")]
    public async Task Login()
    {
    }

    [Authorize]
    [HttpPost("Logout")]
    public async Task Logout()
    {
    }
}