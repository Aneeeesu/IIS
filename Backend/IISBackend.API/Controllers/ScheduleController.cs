using IISBackend.BL.Facades.Interfaces;
using IISBackend.BL.Models.Schedules;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IISBackend.API.Controllers;

[Authorize]
[ApiController]
[Route("schedule")]
public class ScheduleController : ControllerBase
{
    private readonly IScheduleFacade _scheduleFacade;

    public ScheduleController(IScheduleFacade scheduleFacade)
    {
        _scheduleFacade = scheduleFacade;
    }

    [HttpGet]
    public async Task<ActionResult<List<ScheduleDetailModel>>> GetAll()
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

}