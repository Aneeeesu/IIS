using IISBackend.BL.Facades;
using IISBackend.BL.Facades.Interfaces;
using IISBackend.BL.Models.HealthRecords;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IISBackend.API.Controllers;

[ApiController]
[Route("HealthRecords")]
public class HealthRecordController : ControllerBase
{
    private readonly IHealthRecordFacade _healthRecordFacade;

    public HealthRecordController(IHealthRecordFacade healthRecordFacade)
    {
        _healthRecordFacade = healthRecordFacade;
    }

    [HttpGet]
    public async Task<ActionResult<List<HealthRecordListModel>>> GetAll()
    {
        return Ok(await _healthRecordFacade.GetAsync());
    }

    [HttpGet("{recordId}")]
    public async Task<ActionResult<HealthRecordDetailModel>> GetById(Guid recordId)
    {
        var model = await _healthRecordFacade.GetAsync(recordId);

        if (model is not null)
        {
            return Ok(model);
        }

        return NotFound();
    }

    [HttpGet("Animal/{animalId}")]
    public async Task<ActionResult<List<HealthRecordListModel>>> GetAnimalHealthRecords(Guid animalId)
    {
        try
        {
            return Ok(await _healthRecordFacade.GetAnimalRecords(animalId));
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

    [HttpPost("")]
    [Authorize(Roles = "Vet,Admin")]
    public async Task<ActionResult<HealthRecordDetailModel>> Create(HealthRecordCreateModel record)
    {
        try
        {
            var model = await _healthRecordFacade.AuthorizedCreateAsync(record,User);
            return Ok(model);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
        catch (InvalidOperationException)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Error while saving entity");
        }
    }

    [HttpDelete]
    [Authorize(Roles = "Vet,Admin")]
    public async Task<ActionResult> Delete(Guid recordId)
    {
        try
        {
            await _healthRecordFacade.DeleteAsync(recordId);
            return Ok(recordId);
        }
        catch(InvalidOperationException)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Error while deleting entity");
        }
        catch
        {
            return NotFound("ID not found in database");
        }
    }
}