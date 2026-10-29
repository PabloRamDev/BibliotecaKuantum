using biblioteca_kuantum.BusinessLayer.DTO;
using biblioteca_kuantum.DataAccessLayer.Common;

namespace biblioteca_kuantum.BusinessLayer.ServiceContracts
{
    public interface IDocumentService
    {
        public Task<DocumentResponse?> CreateDocumentAsync(CreateDocument document, CancellationToken ct);
        public Task<DocumentResponse?> GetDocumentByIdAsync(int id, CancellationToken ct);
        public Task<PagedResult<DocumentResponse>> FindDocumentAsync(int? id, string? serialCode, string? publicationCode, string searchText, int pageNumber, int pageSize, CancellationToken ct);
        public Task<DocumentResponse?> UpdateDocumentAsync(int id, UpdateDocument document, CancellationToken ct);
        public Task<bool> DeleteDocumentAsync(int id, CancellationToken ct);
    }
}
