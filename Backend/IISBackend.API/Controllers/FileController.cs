using IISBackend.BL.Facades.Interfaces;
using IISBackend.BL.Models.File;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IISBackend.API.Controllers;

[Authorize]
[Route("Files")]
public class FileController : ControllerBase
{
    private readonly IFileUploadFacade _fileFacade;
    public FileController(IFileUploadFacade fileFacade)
    {
        _fileFacade = fileFacade;
    }

    [HttpPost("GenerateUrl")]
    public async Task<ActionResult<PendingFileUploadModel>> GenerateURL(string FileName)
    {
        try
        {
            return Ok(await _fileFacade.GeneratePresignedUrlAsync("mockBucket", FileName, TimeSpan.FromMinutes(5), User));
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

    [HttpPost("ValidateFile/{fileGuid}")]
    public async Task<ActionResult<Guid?>> ValidateFile(Guid fileGuid)
    {
        try
        {
            return Ok(await _fileFacade.ValiadateFileUpload(fileGuid, User));
        }
        catch (UnauthorizedAccessException e)
        {
            return Unauthorized(e.Message);
        }
        catch(Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}