using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital_Management_System.DTOs;
using Hospital_Management_System.Services;

namespace Hospital_Management_System.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Doctor")]
public class XRayAiController : ControllerBase
{
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/png", "image/jpeg", "image/jpg", "image/webp", "image/bmp"
    };

    private readonly IXRayAiService _service;

    public XRayAiController(IXRayAiService service) => _service = service;

    [HttpGet("status")]
    public async Task<ActionResult<XRayAiStatusDto>> Status(CancellationToken cancellationToken)
    {
        var status = await _service.GetStatusAsync(cancellationToken);
        return Ok(status);
    }

    [HttpPost("analyze")]
    [RequestSizeLimit(20 * 1024 * 1024)]
    public async Task<ActionResult<XRayAiAnalyzeResponseDto>> Analyze(
        IFormFile? file,
        [FromForm] string? prompt,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new XRayAiAnalyzeResponseDto { Success = false, Message = "No image file was uploaded." });

        if (file.Length > 20 * 1024 * 1024)
            return BadRequest(new XRayAiAnalyzeResponseDto { Success = false, Message = "File is too large (max 20 MB)." });

        if (!AllowedContentTypes.Contains(file.ContentType))
            return BadRequest(new XRayAiAnalyzeResponseDto { Success = false, Message = "Allowed types: PNG, JPEG, BMP, WebP." });

        await using var stream = file.OpenReadStream();
        var result = await _service.AnalyzeAsync(stream, file.FileName, file.ContentType, prompt, cancellationToken);
        if (!result.Success)
            return StatusCode(503, result);

        return Ok(result);
    }
}
