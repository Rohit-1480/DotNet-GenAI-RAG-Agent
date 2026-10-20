using GenAI.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GenAI.Api.Controllers
{
    [ApiController]
   [Route("api/[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentIngestionService _ingestionService;

        public DocumentController(IDocumentIngestionService ingestionService)
        {
            _ingestionService = ingestionService;
        }

        [HttpPost("ingest")]
        public async Task<IActionResult> Ingest(
        [FromBody] IngestDocumentRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
            {
                return BadRequest("Document text cannot be empty.");
            }

            var chunkCount = await _ingestionService
                .IngestAsync(request.Text);

            return Ok(new
            {
                Message = "Document ingested successfully.",
                ChunksCreated = chunkCount
            });
        }
    }
    public record IngestDocumentRequest(string Text);
}
