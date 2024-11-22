using IISBackend.BL.Facades.Interfaces;
using IISBackend.BL.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IISBackend.API.Controllers;

[Authorize]
[ApiController]
[Route("VerificationRequests")]
public class VerificationRequestController : ControllerBase
{
    private readonly IVerificationRequestFacade _requestFacade;

    public VerificationRequestController(IVerificationRequestFacade requestFacade)
    {
        _requestFacade = requestFacade;
    }

    [HttpGet]
    public async Task<ActionResult<List<VerificationRequestListModel>>> GetAll()
    {
        return Ok(await _requestFacade.GetAsync());
    }

    [HttpGet("{requestId}")]
    public async Task<ActionResult<VerificationRequestDetailModel>> GetById(Guid requestId)
    {
        var model = await _requestFacade.GetAsync(requestId);

        if (model is not null)
        {
            return Ok(model);
        }

        return NotFound();
    }

    [HttpPost(""), Authorize(Roles = "Volunteer")]
    public async Task<ActionResult<VerificationRequestDetailModel?>> Create(VerificationRequestCreateModel request)
    {
        try
        {
            var model = await _requestFacade.AuthorizedCreateAsync(request,User);
            if (model is not null)
            {
                return Ok(model);
            }
            else
            {
                return BadRequest("Request not created");
            }
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
        catch (UnauthorizedAccessException e)
        {
            return Unauthorized(e.Message);
        }
    }

    [HttpDelete("{requestId}")]
    public async Task<ActionResult> Delete(Guid requestId)
    {
        try
        {
            await _requestFacade.AuthorizedDeleteAsync(requestId, User);
            return Ok(requestId);
        }
        catch
        {
            return NotFound("ID not found in database");
        }
    }

    [HttpPost("Resolve/{requestId}"), Authorize(Roles = "Vet,Admin")]
    public async Task<ActionResult<VerificationRequestDetailModel>> ResolveRequest(Guid requestId,bool Approved)
    {
        try
        {
            await _requestFacade.ResolveRequestAsync(requestId, Approved);
                return Ok(requestId);
                
        }catch(InvalidDataException e)
        {
            return BadRequest(e.Message);
        }
        catch (ArgumentException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = e.Message });
        }
    }
}
