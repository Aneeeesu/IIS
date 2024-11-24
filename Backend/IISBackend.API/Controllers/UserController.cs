using IISBackend.BL.Facades.Interfaces;
using IISBackend.BL.Models;
using IISBackend.BL.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IISBackend.API.Controllers;

[ApiController]
[Route("Users")]
public class UserController(IUserFacade userFacade) : ControllerBase
{
    private readonly IUserFacade _userFacade = userFacade;

    [Authorize(Roles = "Admin,Vet")]
    [HttpPost("Caregiver")]
    public async Task<ActionResult<UserDetailModel?>> CreateCaregiver(UserCreateModel model)
    {
        try
        {
            return Created($"/users", await _userFacade.CreateAsync(model, "Caregiver"));
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles= "Admin")]
    [HttpPost("Vet")]
    public async Task<ActionResult<UserDetailModel?>> CreateVet(UserCreateModel model)
    {
        try
        {
            return Created($"/users", await _userFacade.CreateAsync(model, "Vet"));
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize]
    [HttpPut("")]
    public async Task<ActionResult<UserDetailModel?>> EditUser(UserUpdateModel model)
    {
        try
        {
            return Ok(await _userFacade.UpdateAsync(model, User));
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
        catch (UnauthorizedAccessException e)
        {
            return Unauthorized(e.Message);
        }
        catch(InvalidOperationException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpGet("")]
    public async Task<ActionResult<List<UserDetailModel>>> GetAll()
    {
        return Ok(await _userFacade.GetAsync());
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<UserDetailModel>> GetById(Guid userId)
    {
        var model = await _userFacade.GetUserByIdAsync(userId);

        if (model is not null)
        {
            return Ok(model);
        }

        return NotFound();
    }

    [HttpDelete("{userId}")]
    public async Task<ActionResult> Delete(Guid userId)
    {
        try
        {
            await _userFacade.DeleteAsync(userId,User);
            return Ok();
        }
        catch (UnauthorizedAccessException e)
        {
            return Unauthorized(e.Message);
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }

    [Authorize]
    [HttpPut("ChangePassword")]
    public async Task<ActionResult> ChangePassword(ChangePasswordModel newPasswordModel)
    {
        try
        {
            await _userFacade.ChangePasswordAsync(newPasswordModel, User);
            return Ok();
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
        catch (UnauthorizedAccessException e)
        {
            return Unauthorized(e.Message);
        }
        catch (InvalidDataException e)
        {
            return NotFound(e.Message);
        }
    }
}
