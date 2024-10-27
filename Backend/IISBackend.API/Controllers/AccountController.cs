using IISBackend.BL.Facades.Interfaces;
using IISBackend.BL.Models.User;
using IISBackend.DAL.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace IISBackend.API.Controllers;

[ApiController]
[Route("Account")]
public class AccountController(IUserFacade userFacade,UserManager<UserEntity> userMan, SignInManager<UserEntity> signInManager) : ControllerBase
{
    private readonly IUserFacade _userFacade = userFacade;
    private readonly SignInManager<UserEntity> _signInManager = signInManager;

    [HttpPost("Login")]
    public async Task<SignInResult> Login(string name,string password)
    {
        return await _signInManager.PasswordSignInAsync(name, password, false, false);
    }

    [Authorize]
    //[AutoValidateAntiforgeryToken]
    [HttpPost("Logout")]
    public async Task Logout()
    {
        await _signInManager.SignOutAsync();
    }

    [HttpPost("CreateUser")]
    public async Task<ActionResult<UserDetailModel?>> CreateUser(UserCreateModel model)
    {
        try
        {
            return Created($"/Account/GetUsers", await _userFacade.CreateAsync(model));
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize]
    //[AutoValidateAntiforgeryToken]
    [HttpPut("UpdateUser")]
    public async Task<ActionResult<UserDetailModel?>> UpdateUser(UserUpdateModel model)
    {
        try
        {
            return Ok(await _userFacade.UpdateAsync(model,User));
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
        catch(UnauthorizedAccessException e)
        {
            return Unauthorized(e.Message);
        }
    }

    [HttpGet("GetUsers")]
    public async Task<ActionResult<List<UserDetailModel>>> GetUsers()
    {
        try
        {
            return Ok(await _userFacade.GetAsync());
        }
        catch(ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }
}