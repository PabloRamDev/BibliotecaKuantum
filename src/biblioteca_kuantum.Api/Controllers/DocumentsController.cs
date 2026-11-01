using biblioteca_kuantum.BusinessLayer.DTO;
using biblioteca_kuantum.BusinessLayer.ServiceContracts;
using biblioteca_kuantum.DataAccessLayer.Common;
using Microsoft.AspNetCore.Mvc;

namespace biblioteca_kuantum.Api.Controllers
{
    /// <summary>
    /// Biblioteca Kuantum API - Pablo Ramírez.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        public DocumentsController(IDocumentService documentService)
        {
            _documentService = documentService;
        }
        [HttpPost]
        [ProducesResponseType<DocumentResponse>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateDocument([FromBody] CreateDocument document, CancellationToken ct)
        {
            DocumentResponse? response = await _documentService.CreateDocumentAsync(document, ct);

            if (response == null)
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(GetDocumentById), new { id = response.Id }, response);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType<DocumentResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> GetDocumentById(int id, CancellationToken ct)
        {
            DocumentResponse? response = await _documentService.GetDocumentByIdAsync(id, ct);

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }

        [HttpGet("search")]
        [ProducesResponseType<PagedResult<DocumentResponse>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchDocument([FromQuery] int? id, [FromQuery] string? serialCode, [FromQuery] string? publicationCode, [FromQuery] string? searchText, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            PagedResult<DocumentResponse>? response = await _documentService.FindDocumentAsync(id, serialCode, publicationCode, searchText, pageNumber, pageSize, ct);

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType<DocumentResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateDocument(int id, [FromBody] UpdateDocument document, CancellationToken ct)
        {
            DocumentResponse? response = await _documentService.UpdateDocumentAsync(id, document, ct);

            if (response == null)
            {
                return BadRequest();
            }

            return Ok(response);

        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType<DocumentResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteDocument(int id, CancellationToken ct)
        {
            bool? response = await _documentService.DeleteDocumentAsync(id, ct);

            if (response == false)
            {
                return BadRequest();
            }

            return Ok(response);
        }
    }
}
