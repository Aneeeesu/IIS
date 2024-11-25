using IISBackend.BL.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IISBackend.API.Controllers;

[Authorize]
[ApiController]
[Route("ReservationRequests")]
public class ReservationRequestController : ControllerBase
{
    private readonly IReservationRequestFacade _reservationRequestFacade;

    public ReservationRequestController(IReservationRequestFacade reservationRequestFacade)
    {
        _reservationRequestFacade = reservationRequestFacade;
    }

    [HttpGet]
    public async Task<ActionResult<List<ReservationRequestListModel>>> GetAll()
    {
        return Ok(await _reservationRequestFacade.GetAsync());
    }

    [HttpGet("{reservationRequestId}")]
    public async Task<ActionResult<ReservationRequestDetailModel>> GetById(Guid reservationRequestId)
    {
        var model = await _reservationRequestFacade.GetAsync(reservationRequestId);

        if (model is not null)
        {
            return Ok(model);
        }

        return NotFound();
    }

    [HttpPost("")]
    public async Task<ActionResult<ReservationRequestDetailModel?>> Create(ReservationRequestCreateModel reservationRequest)
    {
        try
        {
            var model = await _reservationRequestFacade.AuthorizedCreateRequest(reservationRequest, User);
            if (model is not null)
            {
                return Ok(model);
            }
            else
            {
                return BadRequest("ReservationRequest not created");
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

    [HttpDelete("Cancel/{id}")]
    public async Task<ActionResult<Guid?>> CancelRequest(Guid id)
    {
        try
        {
            await _reservationRequestFacade.AuthorizedCancelRequest(id, User);
            return Ok(id);
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

    [HttpPost("Resolve/{id}")]
    public async Task<ActionResult<ReservationRequestDetailModel?>> ResolveRequest(Guid id,bool Approved)
    {
        try
        {
            var model = await _reservationRequestFacade.AuthorizedResolveRequest(id, Approved, User);
            if (model is not null)
            {
                return Ok(model);
            }
            Ok("Request resolved");
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
        catch(UnauthorizedAccessException e)
        {
            return Unauthorized(e.Message);
        }
        catch(InvalidDataException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}