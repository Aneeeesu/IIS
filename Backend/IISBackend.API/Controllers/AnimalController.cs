using IISBackend.BL.Facades.Interfaces;
using IISBackend.BL.Models;
using Microsoft.AspNetCore.Mvc;

namespace IISBackend.API.Controllers;

[ApiController]
[Route("Animal")]
public class AnimalController : ControllerBase
{
    private readonly IAnimalFacade _animalFacade;

    public AnimalController(IAnimalFacade animalFacade)
    {
        _animalFacade = animalFacade;
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
    public async Task<ActionResult<Guid>> Upsert(AnimalCreateModel animal)
    {
        var guid = await _animalFacade.SaveAsync(animal);
        return Ok(guid);
    }

    [HttpDelete("{animalId}")]
    public async Task<ActionResult> Delete(Guid animalId)
    {
        try
        {
            await _animalFacade.DeleteAsync(animalId);
            return Ok();
        }
        catch
        {
            return NotFound("ID not found in database");
        }
    }
}