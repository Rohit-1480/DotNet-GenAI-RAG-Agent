using Microsoft.AspNetCore.Mvc;
using GenAI.Application.Interfaces;
namespace GenAI.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : Controller
    {
        private readonly IChatService _chatService;
        
        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            var response = await _chatService.GetResponseAsync(request.Message);

            return Ok(new
            {
                response
            });
        }
        public record ChatRequest(string Message);
       
    }
}
