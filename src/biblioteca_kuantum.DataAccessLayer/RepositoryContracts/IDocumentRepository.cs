using biblioteca_kuantum.DataAccessLayer.Common;
using biblioteca_kuantum.DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace biblioteca_kuantum.DataAccessLayer.RepositoryContracts
{
    public interface IDocumentRepository
    {
        public Task<Document?> CreateDocument (Document document, CancellationToken ct); 
        public Task<Document?> GetDocumentById (int id, CancellationToken ct);
        public Task<PagedResult<Document>> FindDocument(int? id, string? serialCode, string? publicationCode, string searchText, int pageNumber, int pageSize, CancellationToken ct);
        public Task<Document?> UpdateDocument(int id, Document document, CancellationToken ct);
        public Task<bool> DeleteDocument(int id, CancellationToken ct);
    }
}
