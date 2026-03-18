namespace Hospital_Management_System.Services
{
    public interface IOpenAIService
    {
        Task<string> GetChatResponseAsync(string userMessage, string context);
    }
}

