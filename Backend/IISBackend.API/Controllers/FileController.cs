using IISBackend.BL.Facades.Interfaces;
using IISBackend.BL.Models.File;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IISBackend.API.Controllers;

[Authorize]
[Route("Files")]
public class FileController : ControllerBase
{
    private readonly IFileFacade _fileFacade;
    public FileController(IFileFacade fileFacade)
    {
        _fileFacade = fileFacade;
    }

    [HttpPost("GenerateUrl")]
    public async Task<ActionResult<PendingFileUploadModel>> GenerateURL(string FileName)
    {
        try
        {
            return Ok(await _fileFacade.GeneratePresignedUrlAsync(FileName, TimeSpan.FromMinutes(5), User));
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
    public async Task<ActionResult<string>> ValidateFile(Guid fileGuid)
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
    [HttpGet("GetAllFiles")]
    [Authorize(Roles ="Admin")]
    public async Task<ActionResult<FileDetailModel>> GetAllFiles()
    {
        return Ok(await _fileFacade.GetFilesAsync());
    }
}