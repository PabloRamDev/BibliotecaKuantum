

using biblioteca_kuantum.DataAccessLayer.Data;
using biblioteca_kuantum.DataAccessLayer.Models;
using biblioteca_kuantum.DataAccessLayer.RepositoryContracts;
using Microsoft.EntityFrameworkCore;


namespace biblioteca_kuantum.DataAccessLayer.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private const string LikeEscapeCharacter = "\\";
        private readonly AppDbContext _dbContext;
        public DocumentRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Document?> CreateDocument(Document document, CancellationToken ct = default)
        {
            if (document == null) 
            {
                return null;
            }

            await _dbContext.Documents.AddAsync(document, ct);

            await _dbContext.SaveChangesAsync();

            return document;

        }

        public async Task<bool> DeleteDocument(int id, CancellationToken ct = default)
        {
            var targetDocument = await _dbContext.Documents.Include(x => x.DocumentPages).FirstOrDefaultAsync(x => x.Id == id, ct);

            if (targetDocument == null)
            {
                throw new ArgumentException("Document not found");
            }

            targetDocument.DeletedAt = DateTime.UtcNow;

            _dbContext.PageIndices.RemoveRange(targetDocument.DocumentPages!);

            await _dbContext.SaveChangesAsync(ct);

            return true;

        }

        public async Task<PagedResult<Document>> FindDocument(int? id, string? serialCode, string? publicationCode, string? searchText, int pageNumber, int pageSize, CancellationToken ct = default)
        {

            IQueryable<Document> query = _dbContext.Documents.AsNoTracking();

            if (id is not null)
            {
                query = query.Where(d => d.Id == id);
            }
            else if (!string.IsNullOrWhiteSpace(serialCode))
            {
                query = query.Where(d => d.SerialCode == serialCode);
            }
            else if (!string.IsNullOrWhiteSpace(publicationCode))
            {
                query = query.Where(d => d.PublicationCode == publicationCode);
            }
            else if (!string.IsNullOrWhiteSpace(searchText))
            {
                var escapedText = searchText
                    .Replace(LikeEscapeCharacter, LikeEscapeCharacter + LikeEscapeCharacter)
                    .Replace("%", LikeEscapeCharacter + "%")
                    .Replace("_", LikeEscapeCharacter + "_");

                var pattern = $"%{escapedText}%";

                query = query.Where(d =>
                    EF.Functions.ILike(d.AuthorFullName, pattern, LikeEscapeCharacter) ||
                    EF.Functions.ILike(d.AuthorEmail, pattern, LikeEscapeCharacter));
            }

            var totalCount = await query.CountAsync(ct);

            var items = await query
                .OrderBy(d => d.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PagedResult<Document>
            {
                Items = items,
                TotalCount = totalCount,
                Page = pageNumber,
                PageSize = pageSize,
            };
        }

        public async Task<Document?> GetDocumentById(int id, CancellationToken ct)
        {
            var existingDocument = await _dbContext.Documents.Include(x => x.DocumentPages).FirstOrDefaultAsync(x => x.Id == id, ct);

            return existingDocument;
        }

        public async Task<Document?> UpdateDocument(int id, Document document, CancellationToken ct)
        {
            var targetDocument = await _dbContext.Documents.Include(x => x.DocumentPages).FirstOrDefaultAsync(x => x.Id == id, ct);

            if (targetDocument == null)
            {
                throw new ArgumentException("Document not found");
            }

            //updated document should replace the Page Indices
            _dbContext.PageIndices.RemoveRange(targetDocument.DocumentPages!);

            DateTimeOffset oldCreatedAt = targetDocument.CreatedAt;

            targetDocument.Name = document.Name;
            targetDocument.Description = document.Description;
            targetDocument.PublicationCode = document.PublicationCode;
            targetDocument.SerialCode = document.SerialCode;
            targetDocument.DocumentPages = document.DocumentPages;
            targetDocument.AuthorFullName = document.AuthorFullName;
            targetDocument.AuthorEmail = document.AuthorEmail;
            targetDocument.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(ct);

            return targetDocument;
        }
    }
}
