using System.Net.Http.Headers;
using System.Text.Json;
using Hospital_Management_System.DTOs;

namespace Hospital_Management_System.Services;

public class XRayAiService : IXRayAiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<XRayAiService> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public XRayAiService(HttpClient httpClient, IConfiguration configuration, ILogger<XRayAiService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    private string? GetBaseUrl()
    {
        var url = _configuration["XRayAi:BaseUrl"]?.Trim();
        return string.IsNullOrWhiteSpace(url) ? null : url.TrimEnd('/');
    }

    public async Task<XRayAiStatusDto> GetStatusAsync(CancellationToken cancellationToken = default)
    {
        var baseUrl = GetBaseUrl();
        if (baseUrl is null)
        {
            return new XRayAiStatusDto
            {
                Success = false,
                ServiceReachable = false,
                Message = "XRayAi:BaseUrl is not configured (e.g. http://127.0.0.1:8000)."
            };
        }

        try
        {
            using var response = await _httpClient.GetAsync($"{baseUrl}/", cancellationToken);
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return new XRayAiStatusDto
                {
                    Success = false,
                    ServiceReachable = true,
                    BaseUrl = baseUrl,
                    Message = $"X-Ray service returned {(int)response.StatusCode}."
                };
            }

            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            var modelLoaded = root.TryGetProperty("modelLoaded", out var ml) && ml.GetBoolean();
            var modelId = root.TryGetProperty("modelId", out var mid) ? mid.GetString() : null;
            var loadErr = root.TryGetProperty("modelLoadError", out var err) && err.ValueKind != JsonValueKind.Null
                ? err.GetString()
                : null;

            return new XRayAiStatusDto
            {
                Success = true,
                ServiceReachable = true,
                BaseUrl = baseUrl,
                ModelLoaded = modelLoaded,
                ModelId = modelId,
                ModelLoadError = loadErr,
                Message = "X-Ray AI service is reachable."
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "X-Ray AI status check failed for {BaseUrl}", baseUrl);
            return new XRayAiStatusDto
            {
                Success = false,
                ServiceReachable = false,
                BaseUrl = baseUrl,
                Message = "Cannot reach X-Ray AI service. Start Model/run.ps1 (FastAPI on port 8000)."
            };
        }
    }

    public async Task<XRayAiAnalyzeResponseDto> AnalyzeAsync(
        Stream imageStream,
        string fileName,
        string? contentType,
        string? prompt,
        CancellationToken cancellationToken = default)
    {
        var baseUrl = GetBaseUrl();
        if (baseUrl is null)
        {
            return new XRayAiAnalyzeResponseDto
            {
                Success = false,
                Message = "XRayAi:BaseUrl is not configured."
            };
        }

        using var form = new MultipartFormDataContent();
        var streamContent = new StreamContent(imageStream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(
            string.IsNullOrWhiteSpace(contentType) ? "image/jpeg" : contentType);
        form.Add(streamContent, "image", fileName);

        if (!string.IsNullOrWhiteSpace(prompt))
            form.Add(new StringContent(prompt.Trim()), "prompt");

        try
        {
            using var response = await _httpClient.PostAsync($"{baseUrl}/analyze", form, cancellationToken);
            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("X-Ray analyze failed {Status}: {Body}", (int)response.StatusCode, body.Length > 500 ? body[..500] : body);
                var message = TryReadErrorDetail(body) ?? $"X-Ray service error ({(int)response.StatusCode}).";
                return new XRayAiAnalyzeResponseDto { Success = false, Message = message };
            }

            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            var report = root.TryGetProperty("report", out var r) ? r.GetString() : body;
            var modelId = root.TryGetProperty("modelId", out var m) ? m.GetString() : null;
            var usedPrompt = root.TryGetProperty("prompt", out var p) ? p.GetString() : null;

            return new XRayAiAnalyzeResponseDto
            {
                Success = true,
                Report = report,
                ModelId = modelId,
                Prompt = usedPrompt,
                Message = "Analysis completed."
            };
        }
        catch (TaskCanceledException)
        {
            return new XRayAiAnalyzeResponseDto
            {
                Success = false,
                Message = "Analysis timed out. The vision model may still be loading — try again."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "X-Ray analyze request failed");
            return new XRayAiAnalyzeResponseDto
            {
                Success = false,
                Message = "Failed to call X-Ray AI service. Ensure FastAPI is running (Model/run.ps1)."
            };
        }
    }

    private static string? TryReadErrorDetail(string body)
    {
        try
        {
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("detail", out var detail))
            {
                if (detail.ValueKind == JsonValueKind.String)
                    return detail.GetString();
                if (detail.TryGetProperty("details", out var details))
                    return details.GetString();
                if (detail.TryGetProperty("error", out var err))
                    return err.GetString();
            }
        }
        catch
        {
            // ignore
        }

        return null;
    }
}
