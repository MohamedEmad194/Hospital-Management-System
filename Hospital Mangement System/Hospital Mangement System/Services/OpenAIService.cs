using System.Text;
using System.Text.Json;

namespace Hospital_Management_System.Services
{
    public class OpenAIService : IOpenAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;
        private readonly string _apiUrl;
        private readonly ILogger<OpenAIService> _logger;

        public OpenAIService(IConfiguration configuration, ILogger<OpenAIService> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
            _apiKey = configuration["ChatbotSettings:OpenAIApiKey"] ?? "";
            _model = configuration["ChatbotSettings:OpenAIModel"] ?? "gpt-3.5-turbo";
            _apiUrl = configuration["ChatbotSettings:ApiUrl"] ?? "https://api.openai.com/v1/chat/completions";
            
            if (!string.IsNullOrEmpty(_apiKey))
            {
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
            }
        }

        public async Task<string> GetChatResponseAsync(string userMessage, string context)
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                _logger.LogWarning("OpenAI API key is not configured");
                return string.Empty;
            }

            try
            {
                var systemPrompt = $"أنت مساعد ذكي لمستشفى الحياة. يجب أن ترد بالعربية دائماً. {context}\n\n" +
                                   "قواعد مهمة:\n" +
                                   "1. دائماً رد بالعربية\n" +
                                   "2. كن مفيداً ومهذباً\n" +
                                   "3. قدم معلومات دقيقة عن المستشفى قدر الإمكان\n" +
                                   "4. اقترح الأطباء والمواعيد المناسبة عندما يكون ذلك مناسباً\n" +
                                   "5. إذا لم تعرف الإجابة أو لم تكن متأكداً، اعترف بذلك ووجّه المستخدم للاستقبال أو الأقسام المختصة";

                var requestBody = new
                {
                    model = _model,
                    messages = new[]
                    {
                        new { role = "system", content = systemPrompt },
                        new { role = "user", content = userMessage }
                    },
                    temperature = 0.7,
                    max_tokens = 500
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_apiUrl, content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var responseJson = JsonDocument.Parse(responseContent);

                var aiResponse = responseJson.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                return aiResponse ?? "عذراً، لم أتمكن من الحصول على رد. يرجى المحاولة مرة أخرى.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling OpenAI API");
                return string.Empty; // Return empty to fallback to rule-based response
            }
        }
    }
}

