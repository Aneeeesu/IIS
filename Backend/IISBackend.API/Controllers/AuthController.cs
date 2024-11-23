using IISBackend.BL.Facades.Interfaces;
using IISBackend.BL.Models;
using IISBackend.BL.Models.User;
using IISBackend.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace IISBackend.API.Controllers;

[ApiController]
[Route("Account")]
public class AuthController(IUserFacade userFacade) : ControllerBase
{

    [HttpPost("Login")]
    public async Task<SignInResult> Login(LoginModel login)
    {
        return await userFacade.Login(login);
    }

    [Authorize]
    //[AutoValidateAntiforgeryToken]
    [HttpPost("Logout")]
    public async Task Logout()
    {
        await userFacade.Logout();
    }

    [Authorize]
    [HttpGet("GetUserID")]
    public Guid? GetUserID()
    {
        return userFacade.GetCurrentUserGuid(User);
    }

    [HttpPost("Register")]
    public async Task<ActionResult<UserDetailModel?>> CreateUser(UserCreateModel model)
    {
        try
        {
            return Created($"/Account/GetUsers", await userFacade.CreateAsync(model,"Volunteer"));
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }
}