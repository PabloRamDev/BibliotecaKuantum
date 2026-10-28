using biblioteca_kuantum.DataAccessLayer.Common;
using biblioteca_kuantum.DataAccessLayer.Models;
using biblioteca_kuantum.DataAccessLayer.RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace biblioteca_kuantum.BusinessLayer.Services
{
    public class DocumentService : IDocumentRepository
    {
        public DocumentService()
        {
            
        }
        public Task<Document?> CreateDocument(Document document, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteDocument(int id, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<Document>> FindDocument(int? id, string? serialCode, string? publicationCode, string searchText, int pageNumber, int pageSize, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<Document?> GetDocumentById(int id, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<Document?> UpdateDocument(int id, Document document, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
