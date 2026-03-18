using Hospital_Management_System.DTOs;

namespace Hospital_Management_System.Services
{
    public interface IChatbotService
    {
        Task<ChatbotResponseDto> ProcessMessageAsync(ChatbotMessageDto message);
    }
}

