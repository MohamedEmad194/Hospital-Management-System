using Hospital_Management_System.DTOs;

namespace Hospital_Management_System.Services;

public interface IXRayAiService
{
    Task<XRayAiStatusDto> GetStatusAsync(CancellationToken cancellationToken = default);
    Task<XRayAiAnalyzeResponseDto> AnalyzeAsync(
        Stream imageStream,
        string fileName,
        string? contentType,
        string? prompt,
        CancellationToken cancellationToken = default);
}
