using biblioteca_kuantum.BusinessLayer.DTO;
using biblioteca_kuantum.BusinessLayer.ServiceContracts;
using biblioteca_kuantum.DataAccessLayer.Common;
using biblioteca_kuantum.DataAccessLayer.Models;


namespace biblioteca_kuantum.BusinessLayer.Services
{
    public class DocumentService : IDocumentService
    {
        public DocumentService()
        {
            
        }

        public Task<DocumentResponse?> CreateDocumentAsync(CreateDocument document, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteDocumentAsync(int id, CancellationToken ct)
        {
            throw new NotImplementedException();
        }


        public Task<PagedResult<DocumentResponse>> FindDocumentAsync(int? id, string? serialCode, string? publicationCode, string searchText, int pageNumber, int pageSize, CancellationToken ct)
        {
            throw new NotImplementedException();
        }


        public Task<DocumentResponse?> GetDocumentByIdAsync(int id, CancellationToken ct)
        {
            throw new NotImplementedException();
        }


        public Task<DocumentResponse?> UpdateDocumentAsync(int id, UpdateDocument document, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
