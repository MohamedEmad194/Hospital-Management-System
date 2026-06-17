namespace Hospital_Management_System.DTOs;

public class XRayAiStatusDto
{
    public bool Success { get; set; }
    public bool ServiceReachable { get; set; }
    public string? BaseUrl { get; set; }
    public bool ModelLoaded { get; set; }
    public string? ModelId { get; set; }
    public string? Message { get; set; }
    public string? ModelLoadError { get; set; }
}

public class XRayAiAnalyzeResponseDto
{
    public bool Success { get; set; }
    public string? ModelId { get; set; }
    public string? Report { get; set; }
    public string? Prompt { get; set; }
    public string? Message { get; set; }
}
