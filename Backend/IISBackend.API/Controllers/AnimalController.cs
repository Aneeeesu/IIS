using IISBackend.BL.Facades.Interfaces;
using IISBackend.BL.Models.Animal;
using IISBackend.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IISBackend.API.Controllers;

[ApiController]
[Route("Animal")]
public class AnimalController : ControllerBase
{
    private readonly IAnimalFacade _animalFacade;
    private readonly IAnimalStatusFacade _animalStatusFacade;
    public AnimalController(IAnimalFacade animalFacade, IAnimalStatusFacade  animalStatusFacade)
    {
        _animalFacade = animalFacade;
        _animalStatusFacade = animalStatusFacade;
    }

    [HttpGet("")]
    public async Task<ActionResult<List<AnimalListModel>>> GetAll()
    {
        return Ok(await _animalFacade.GetAsync());
    }

    [HttpGet("{animalId}")]
    public async Task<ActionResult<AnimalDetailModel>> GetById(Guid animalId)
    {
        var model = await _animalFacade.GetAsync(animalId);

        if (model is not null)
        {
            return Ok(model);
        }

        return NotFound();
    }

    [HttpPost("")]
    [Authorize(Roles = "Admin,Vet,Caregiver")]
    public async Task<ActionResult<AnimalDetailModel>> Upsert(AnimalCreateModel animal)
    {
        try
        {
            var detailModel = await _animalFacade.SaveAsync(animal);
            return Ok(detailModel);
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

    [HttpDelete("{animalId}")]
    [Authorize(Roles = "Admin,Vet,Caregiver")]
    public async Task<ActionResult> Delete(Guid animalId)
    {
        try
        {
            await _animalFacade.DeleteAsync(animalId);
            return Ok(animalId);
        }
        catch
        {
            return NotFound("ID not found in database");
        }
    }

    [HttpPost("Status")]
    [Authorize(Roles = "Admin,Vet,Caregiver")]
    public async Task<ActionResult<AnimalStatusDetailModel>> AddStatus(AnimalStatusCreateModel model)
    {
        try
        {
            return await _animalStatusFacade.CreateAsync(model);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
        catch (InvalidOperationException)
        {
            throw new InvalidOperationException("Error while saving entity");
        }
    }
}
