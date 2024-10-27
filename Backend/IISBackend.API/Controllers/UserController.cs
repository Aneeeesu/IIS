using IISBackend.BL.Facades.Interfaces;
using IISBackend.BL.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IISBackend.API.Controllers;

[ApiController]
[Route("users")]
public class UserController(IUserFacade userFacade) : ControllerBase
{
    private readonly IUserFacade _userFacade = userFacade;

    [Authorize(Roles = "Admin,Vet")]
    [HttpPost("caretakers")]
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
    [HttpPost("vets")]
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
            await _userFacade.DeleteAsync(userId);
            return Ok();
        }
        catch
        {
            return NotFound("ID not found in database");
        }
    }
}