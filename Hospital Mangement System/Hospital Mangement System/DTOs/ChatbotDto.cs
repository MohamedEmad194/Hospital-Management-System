using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Hospital_Management_System.DTOs
{
    public class ChatbotMessageDto
    {
        [Required]
        [StringLength(1000)]
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }

    public class ChatbotResponseDto
    {
        public string Response { get; set; } = string.Empty;
        public List<string>? Suggestions { get; set; }
    }
}

