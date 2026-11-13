using GenAI.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GenAI.Api.Controllers
{
    [ApiController]
   [Route("api/[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentIngestionService _ingestionService;
        private readonly IDocumentManagementService _documentManagementService;

        public DocumentController(IDocumentIngestionService ingestionService, IDocumentManagementService documentManagementService)
        {
            _ingestionService = ingestionService;
            _documentManagementService = documentManagementService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllDocuments()
        {
            var documents = await _documentManagementService
                .GetAllDocumentsAsync();

            return Ok(documents);
        }
        [HttpPost("ingest")]
        public async Task<IActionResult> Ingest(
        [FromBody] IngestDocumentRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
            {
                return BadRequest("Document text cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(request.Source))
            {
                return BadRequest("Document source cannot be empty.");
            }
            var chunkCount = await _ingestionService
                .IngestAsync(request.Text, request.Source);

            return Ok(new
            {
                Message = "Document ingested successfully.",
                Source = request.Source,
                ChunksCreated = chunkCount
            });
        }
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetDocumentById(Guid id)
        {
            var document = await _documentManagementService
                .GetDocumentByIdAsync(id);

            if (document == null)
            {
                return NotFound(new
                {
                    Message = $"Document with ID {id} was not found."
                });
            }

            return Ok(document);
        }
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteDocumentById(Guid id)
        {
            var isDeleted = await _documentManagementService
                .DeleteDocumentByIdAsync(id);
            if (!isDeleted)
            {
                return NotFound(new
                {
                    Message = $"Document with ID {id} was not found."
                });
            }
            return Ok(new
            {
                Message = $"Document with ID {id} has been deleted successfully."
            });
        }
    }
    public record IngestDocumentRequest(string Text, string Source);
}
