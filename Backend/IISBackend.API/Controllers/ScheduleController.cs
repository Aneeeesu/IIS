﻿using IISBackend.BL.Facades.Interfaces;
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
    public async Task<ActionResult<ScheduleDetailModel?>> Create(ScheduleCreateModel schedule)
    {
        try
        {
            var model = await _scheduleFacade.AuthorizedCreateAsync(schedule,User);
            return Ok(model);
        }
        catch (ArgumentException e)
        {
            return BadRequest( e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
    }


    [HttpDelete("{scheduleId}")]
    public async Task<ActionResult<Guid?>> Delete(Guid scheduleId)
    {
        try
        {
            await _scheduleFacade.AuthorizedCancelAsync(scheduleId, User);
            return Ok(scheduleId);
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

    [HttpGet("Animal/{animalID}")]
    public async Task<ActionResult<List<ScheduleListModel>>> GetAnimalSchedules(Guid animalID)
    {
        try
        {
            return Ok(await _scheduleFacade.GetAnimalSchedulesAsync(animalID));
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

    [HttpGet("User/{userID}")]
    public async Task<ActionResult<List<ScheduleListModel>>> GetUserSchedules(Guid userID)
    {
        try
        {
            return Ok(await _scheduleFacade.GetUserSchedulesAsync(userID));
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
