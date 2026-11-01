using biblioteca_kuantum.BusinessLayer.DTO;
using biblioteca_kuantum.BusinessLayer.ServiceContracts;
using biblioteca_kuantum.BusinessLayer.Tests.Helpers;
using biblioteca_kuantum.DataAccessLayer.Data;
using biblioteca_kuantum.DataAccessLayer.Models;
using biblioteca_kuantum.DataAccessLayer.Repositories;
using biblioteca_kuantum.DataAccessLayer.RepositoryContracts;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

//Test Suite Generated With Claude
namespace biblioteca_kuantum.BusinessLayer.Tests.Services
{
    public class DocumentServiceTests : IDisposable
    {
        private static readonly CancellationToken Ct = CancellationToken.None;

        // Seed contains 6 documents; 1 of them is soft-deleted.
        private const int ActiveSeedDocuments = 5;

        private readonly AppDbContext _context;
        private readonly ServiceProvider _provider;
        private readonly IDocumentService _service;

        // xUnit creates a new instance per test, so every test gets its own seeded database.
        public DocumentServiceTests()
        {
            _context = TestDbContextFactory.CreateInMemoryDbContext();

            var services = new ServiceCollection();
            services.AddLogging();
            services.AddBusinessLayer();
            services.AddSingleton(_context);
            services.AddScoped<IDocumentRepository, DocumentRepository>();

            _provider = services.BuildServiceProvider();
            _service = _provider.GetRequiredService<IDocumentService>();
        }

        public void Dispose()
        {
            _provider.Dispose();
            _context.Dispose();
        }

        // ---------------------------------------------------------------
        // Helpers
        // ---------------------------------------------------------------

        /// <summary>Ids are generated, so tests look documents up by their unique serial code.</summary>
        private async Task<int> GetIdBySerialCode(string serialCode) =>
            (await _context.Documents
                .IgnoreQueryFilters()
                .AsNoTracking()
                .SingleAsync(d => d.SerialCode == serialCode, Ct)).Id;

        /// <summary>Forgets tracked entities so the next query reads what was actually saved.</summary>
        private void ResetTracking() => _context.ChangeTracker.Clear();

        private static CreateDocument ValidCreateRequest() => new()
        {
            Name = "Manual de Pruebas",
            Description = "Documento creado desde los tests.",
            AuthorFullName = "Tester Uno",
            AuthorEmail = "tester@example.com",
            SerialCode = "ABCDEF0123456789",
            PublicationCode = "ISO-14001",
            DocumentPages =
            [
                new CreateDocumentPageIndex { Name = "Inicio", Page = 1 },
                new CreateDocumentPageIndex { Name = "Anexos", Page = 9 },
            ]
        };

        private static UpdateDocument ValidUpdateRequest() => new()
        {
            Name = "Protección de Datos Personales (actualizado)",
            Description = "Descripción actualizada.",
            AuthorFullName = "Camila Rojas",
            AuthorEmail = "camila.rojas@example.com",
            SerialCode = "DEADBEEF00112233",
            PublicationCode = "Ley N\u00B0 19.628",
        };

        // ---------------------------------------------------------------
        // CreateDocumentAsync
        // ---------------------------------------------------------------

        [Fact]
        public async Task CreateDocumentAsync_ValidRequest_ReturnsResponseWithGeneratedId()
        {
            var result = await _service.CreateDocumentAsync(ValidCreateRequest(), Ct);

            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.Equal("ABCDEF0123456789", result.SerialCode);
            Assert.Equal("ISO-14001", result.PublicationCode);
        }

        [Fact]
        public async Task CreateDocumentAsync_WithInvalidEmail_ThrowsValidationError()
        {
            CreateDocument request = ValidCreateRequest();
            request.AuthorEmail = "invalidemail@email";

            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => _service.CreateDocumentAsync(request, Ct));

            Assert.Contains(exception.Errors,
                e => e.PropertyName == nameof(CreateDocument.AuthorEmail));

        }


        [Fact]
        public async Task CreateDocumentAsync_ValidRequest_PersistsDocumentWithPages()
        {
            await _service.CreateDocumentAsync(ValidCreateRequest(), Ct);
            ResetTracking();

            var saved = await _context.Documents
                .Include(d => d.DocumentPages)
                .SingleAsync(d => d.SerialCode == "ABCDEF0123456789", Ct);

            Assert.Equal(2, saved.DocumentPages!.Count);
            Assert.All(saved.DocumentPages, p => Assert.Equal(saved.Id, p.DocumentId));
            Assert.Equal(ActiveSeedDocuments + 1, await _context.Documents.CountAsync(Ct));
        }

        [Theory]
        [InlineData("ISO27001")]          // missing dash
        [InlineData("Ley N\u00B0 19628")] // missing thousands separator
        [InlineData("P-01.20251314")]     // month 13
        [InlineData("P-01.20250230")]     // February 30th
        [InlineData("RANDOM-CODE")]
        public async Task CreateDocumentAsync_InvalidPublicationCode_ThrowsAndDoesNotPersist(string publicationCode)
        {
            var request = ValidCreateRequest();
            request.PublicationCode = publicationCode;

            await Assert.ThrowsAsync<ValidationException>(() => _service.CreateDocumentAsync(request, Ct));

            Assert.Equal(ActiveSeedDocuments, await _context.Documents.CountAsync(Ct));
        }

        [Theory]
        [InlineData("ABCDEF01234567890")] // 17 characters
        [InlineData("GHIJKL0123456789")]  // non-hex characters
        [InlineData("")]                  // empty
        public async Task CreateDocumentAsync_InvalidSerialCode_Throws(string serialCode)
        {
            var request = ValidCreateRequest();
            request.SerialCode = serialCode;

            await Assert.ThrowsAsync<ValidationException>(() => _service.CreateDocumentAsync(request, Ct));
        }

        [Theory]
        [InlineData("A")]                 // minimum
        [InlineData("ABCDEF012345678")]   // 15 characters
        [InlineData("ABCDEF0123456789")]  // maximum
        public async Task CreateDocumentAsync_SerialCodeUpTo16HexCharacters_IsAccepted(string serialCode)
        {
            var request = ValidCreateRequest();
            request.SerialCode = serialCode;

            var result = await _service.CreateDocumentAsync(request, Ct);

            Assert.NotNull(result);
        }
        // ---------------------------------------------------------------
        // GetDocumentByIdAsync
        // ---------------------------------------------------------------

        [Fact]
        public async Task GetDocumentByIdAsync_ExistingDocument_ReturnsIt()
        {
            var id = await GetIdBySerialCode("DEADBEEF00112233");

            var result = await _service.GetDocumentByIdAsync(id, Ct);

            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Ley N\u00B0 19.628", result.PublicationCode);
            Assert.Equal("Camila Rojas", result.AuthorFullName);
        }

        [Fact]
        public async Task GetDocumentByIdAsync_NonExistingId_ReturnsNull()
        {
            var result = await _service.GetDocumentByIdAsync(99999, Ct);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetDocumentByIdAsync_SoftDeletedDocument_ReturnsNull()
        {
            var id = await GetIdBySerialCode("00FF00FF00FF00FF");

            var result = await _service.GetDocumentByIdAsync(id, Ct);

            Assert.Null(result);
        }

        // ---------------------------------------------------------------
        // FindDocumentAsync
        // ---------------------------------------------------------------

        [Fact]
        public async Task FindDocumentAsync_NoFilters_ReturnsAllActiveDocuments()
        {
            var result = await _service.FindDocumentAsync(null, null, null, string.Empty, 1, 50, Ct);

            Assert.Equal(ActiveSeedDocuments, result.TotalCount);
            Assert.Equal(ActiveSeedDocuments, result.Items.Count);
            Assert.DoesNotContain(result.Items, d => d.SerialCode == "00FF00FF00FF00FF");
        }

        [Fact]
        public async Task FindDocumentAsync_ById_ReturnsSingleDocument()
        {
            var id = await GetIdBySerialCode("A1B2C3D4E5F60718");

            var result = await _service.FindDocumentAsync(id, null, null, string.Empty, 1, 10, Ct);

            Assert.Equal(1, result.TotalCount);
            Assert.Equal("ISO-27001", result.Items.Single().PublicationCode);
        }

        [Fact]
        public async Task FindDocumentAsync_BySerialCode_ReturnsSingleDocument()
        {
            var result = await _service.FindDocumentAsync(null, "1234567890ABCDEF", null, string.Empty, 1, 10, Ct);

            Assert.Equal(1, result.TotalCount);
            Assert.Equal("P-01.20250614", result.Items.Single().PublicationCode);
        }

        [Fact]
        public async Task FindDocumentAsync_ByPublicationCode_ReturnsSingleDocument()
        {
            var result = await _service.FindDocumentAsync(null, null, "Ley N\u00B0 19.628", string.Empty, 1, 10, Ct);

            Assert.Equal(1, result.TotalCount);
            Assert.Equal("DEADBEEF00112233", result.Items.Single().SerialCode);
        }

        [Fact]
        public async Task FindDocumentAsync_UnknownSerialCode_ReturnsEmptyResult()
        {
            var result = await _service.FindDocumentAsync(null, "FFFFFFFFFFFFFFFF", null, string.Empty, 1, 10, Ct);

            Assert.Equal(0, result.TotalCount);
            Assert.Empty(result.Items);
        }

        [Fact]
        public async Task FindDocumentAsync_SoftDeletedSerialCode_ReturnsEmptyResult()
        {
            var result = await _service.FindDocumentAsync(null, "00FF00FF00FF00FF", null, string.Empty, 1, 10, Ct);

            Assert.Equal(0, result.TotalCount);
            Assert.Empty(result.Items);
        }

        [Fact]
        public async Task FindDocumentAsync_Paging_ReturnsRequestedPageAndTotalCount()
        {
            var page1 = await _service.FindDocumentAsync(null, null, null, string.Empty, 1, 2, Ct);
            var page3 = await _service.FindDocumentAsync(null, null, null, string.Empty, 3, 2, Ct);

            Assert.Equal(ActiveSeedDocuments, page1.TotalCount);
            Assert.Equal(2, page1.Items.Count);
            Assert.Equal(1, page1.Page);
            Assert.Equal(2, page1.PageSize);

            Assert.Single(page3.Items);   // 5 documents, page size 2 => last page has 1
            Assert.Empty(page1.Items.Select(d => d.Id).Intersect(page3.Items.Select(d => d.Id)));
        }

        [Fact]
        public async Task FindDocumentAsync_PageBeyondResults_ReturnsEmptyItemsButKeepsTotalCount()
        {
            var result = await _service.FindDocumentAsync(null, null, null, string.Empty, 10, 10, Ct);

            Assert.Empty(result.Items);
            Assert.Equal(ActiveSeedDocuments, result.TotalCount);
        }

        [Fact(Skip = "EF.Functions.ILike is PostgreSQL-only and is not supported by the InMemory provider. Cover with a Testcontainers test.")]
        public async Task FindDocumentAsync_BySearchText_MatchesAuthorNameOrEmail()
        {
            var result = await _service.FindDocumentAsync(null, null, null, "rojas", 1, 10, Ct);

            Assert.Equal(2, result.TotalCount);
        }

        // ---------------------------------------------------------------
        // UpdateDocumentAsync
        // ---------------------------------------------------------------

        [Fact]
        public async Task UpdateDocumentAsync_ExistingDocument_ReturnsUpdatedResponse()
        {
            var id = await GetIdBySerialCode("DEADBEEF00112233");

            var result = await _service.UpdateDocumentAsync(id, ValidUpdateRequest(), Ct);

            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Protección de Datos Personales (actualizado)", result.Name);
        }

        [Fact]
        public async Task UpdateDocumentAsync_ExistingDocument_PersistsChangesAndKeepsPages()
        {
            var id = await GetIdBySerialCode("DEADBEEF00112233");

            await _service.UpdateDocumentAsync(id, ValidUpdateRequest(), Ct);
            ResetTracking();

            var saved = await _context.Documents
                .Include(d => d.DocumentPages)
                .SingleAsync(d => d.Id == id, Ct);

            Assert.Equal("Descripción actualizada.", saved.Description);
            Assert.Equal(3, saved.DocumentPages!.Count);   // update must not touch the pages
        }

        [Fact]
        public async Task UpdateDocumentAsync_NonExistingId_ReturnsNull()
        {
            var result = await _service.UpdateDocumentAsync(99999, ValidUpdateRequest(), Ct);

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateDocumentAsync_SoftDeletedDocument_ReturnsNull()
        {
            var id = await GetIdBySerialCode("00FF00FF00FF00FF");

            var result = await _service.UpdateDocumentAsync(id, ValidUpdateRequest(), Ct);

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateDocumentAsync_InvalidRequest_ThrowsAndDoesNotPersist()
        {
            var id = await GetIdBySerialCode("DEADBEEF00112233");
            var request = ValidUpdateRequest();
            request.PublicationCode = "ISO27001";

            await Assert.ThrowsAsync<ValidationException>(() => _service.UpdateDocumentAsync(id, request, Ct));

            ResetTracking();
            var saved = await _context.Documents.SingleAsync(d => d.Id == id, Ct);
            Assert.Equal("Ley N\u00B0 19.628", saved.PublicationCode);
        }

        // ---------------------------------------------------------------
        // DeleteDocumentAsync
        // ---------------------------------------------------------------

        [Fact]
        public async Task DeleteDocumentAsync_ExistingDocument_ReturnsTrue()
        {
            var id = await GetIdBySerialCode("A1B2C3D4E5F60718");

            var deleted = await _service.DeleteDocumentAsync(id, Ct);

            Assert.True(deleted);
        }

        [Fact]
        public async Task DeleteDocumentAsync_ExistingDocument_SoftDeletesDocumentAndRemovesPages()
        {
            var id = await GetIdBySerialCode("A1B2C3D4E5F60718");

            await _service.DeleteDocumentAsync(id, Ct);
            ResetTracking();

            var document = await _context.Documents
                .IgnoreQueryFilters()
                .SingleAsync(d => d.Id == id, Ct);
            var remainingPages = await _context.Set<DocumentPageIndex>()
                .CountAsync(p => p.DocumentId == id, Ct);

            Assert.NotNull(document.DeletedAt);   // row still exists, marked as deleted
            Assert.Equal(0, remainingPages);      // pages are physically removed
            Assert.Null(await _service.GetDocumentByIdAsync(id, Ct));
        }

        [Fact]
        public async Task DeleteDocumentAsync_NonExistingId_ReturnsFalse()
        {
            var deleted = await _service.DeleteDocumentAsync(99999, Ct);

            Assert.False(deleted);
        }

        [Fact]
        public async Task DeleteDocumentAsync_AlreadyDeletedDocument_ReturnsFalse()
        {
            var id = await GetIdBySerialCode("00FF00FF00FF00FF");

            var deleted = await _service.DeleteDocumentAsync(id, Ct);

            Assert.False(deleted);
        }
    }
}