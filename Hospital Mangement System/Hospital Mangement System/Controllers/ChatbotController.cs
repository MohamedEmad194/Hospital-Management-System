using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital_Management_System.DTOs;
using Hospital_Management_System.Services;

namespace Hospital_Management_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class ChatbotController : ControllerBase
    {
        private readonly IChatbotService _chatbotService;
        private readonly ILogger<ChatbotController> _logger;

        public ChatbotController(IChatbotService chatbotService, ILogger<ChatbotController> logger)
        {
            _chatbotService = chatbotService;
            _logger = logger;
        }

        /// <summary>
        /// Send a message to the chatbot and get a response
        /// </summary>
        [HttpPost("message")]
        public async Task<ActionResult<ChatbotResponseDto>> SendMessage([FromBody] ChatbotMessageDto message)
        {
            try
            {
                _logger.LogInformation("Received chatbot message: {Message}", message?.Message);

                if (message == null)
                {
                    _logger.LogWarning("Chatbot message is null");
                    return BadRequest(new { message = "Message is required" });
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Model state is invalid: {Errors}", string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                    return BadRequest(new { message = "Invalid request", errors = ModelState });
                }

                if (string.IsNullOrWhiteSpace(message.Message))
                {
                    _logger.LogWarning("Message is empty");
                    return BadRequest(new { message = "Message cannot be empty" });
                }

                var response = await _chatbotService.ProcessMessageAsync(message);
                _logger.LogInformation("Chatbot response generated successfully");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chatbot message");
                return StatusCode(500, new { message = "An error occurred while processing your message", error = ex.Message });
            }
        }
    }
}

