using IISBackend.BL.Facades.Interfaces;
using IISBackend.BL.Models.Schedules;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IISBackend.API.Controllers;

[Authorize]
[ApiController]
[Route("Schedule")]
public class ScheduleController : ControllerBase
{
    private readonly IScheduleFacade _scheduleFacade;

    public ScheduleController(IScheduleFacade scheduleFacade)
    {
        _scheduleFacade = scheduleFacade;
    }

    [HttpGet]
    public async Task<ActionResult<List<ScheduleListModel>>> GetAll()
    {
        return Ok(await _scheduleFacade.GetAsync());
    }

    [HttpGet("{scheduleId}")]
    public async Task<ActionResult<ScheduleDetailModel>> GetById(Guid scheduleId)
    {
        var model = await _scheduleFacade.GetAsync(scheduleId);

        if (model is not null)
        {
            return Ok(model);
        }

        return NotFound();
    }

    [HttpPost("")]
    [Authorize(Roles= "Admin,Vet")]
    public async Task<ActionResult<ScheduleDetailModel?>> Create(ScheduleCreateModel schedule)
    {
        try
        {
            var model = await _scheduleFacade.CreateAsync(schedule);
            if (model is not null)
            {
                return Ok(model);
            }
            else {
                return BadRequest("Schedule not created");
            }
        }
        catch(ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }


    [HttpDelete("{scheduleId}")]
    public async Task<ActionResult> Delete(Guid scheduleId)
    {
        try
        {
            await _scheduleFacade.AuthorizedDeleteAsync(scheduleId,User);
            return Ok();
        }
        catch
        {
            return NotFound("ID not found in database");
        }
    }

    [HttpGet("Animal/{animalID}")]
    public async Task<ActionResult<List<ScheduleListModel>>> GetAnimalSchedules(Guid animalID)
    {
        return Ok(await _scheduleFacade.GetAnimalSchedulesAsync(animalID));
    }

    [HttpGet("Volunteer/{volunteerID}")]
    public async Task<ActionResult<List<ScheduleListModel>>> GetVolunteerSchedules(Guid volunteerID)
    {
        return Ok(await _scheduleFacade.GetVolunteerSchedulesAsync(volunteerID));
    }
}