using IISBackend.BL.Facades.Interfaces;
using IISBackend.BL.Models.User;
using IISBackend.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mysqlx.Crud;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace IISBackend.API.Controllers;

[ApiController]
[Route("Account")]
public class AuthController(IUserFacade userFacade,UserManager<UserEntity> userMan, SignInManager<UserEntity> signInManager) : ControllerBase
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

    [HttpPost("Register")]
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
}