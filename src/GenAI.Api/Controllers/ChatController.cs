using Microsoft.AspNetCore.Mvc;
using GenAI.Application.Interfaces;
namespace GenAI.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : Controller
    {
        private readonly IChatService _chatService;
        private readonly IContextProvider _contextProvider;
        public ChatController(IChatService chatService , IContextProvider contextProvider)
        {
            _chatService = chatService;
            _contextProvider = contextProvider;
        }

        [HttpPost]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            var context = await _contextProvider.GetContextAsync(request.Message);
            var response = await _chatService.GetResponseAsync(request.Message,context);

            return Ok(new
            {
                response
            });
        }
        public record ChatRequest(string Message);
        //public record ChatRequest(string Message, string? Context);

    }
}
