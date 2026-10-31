using AutoMapper;
using biblioteca_kuantum.BusinessLayer.DTO;
using biblioteca_kuantum.BusinessLayer.ServiceContracts;
using biblioteca_kuantum.DataAccessLayer.Common;
using biblioteca_kuantum.DataAccessLayer.Models;
using biblioteca_kuantum.DataAccessLayer.RepositoryContracts;
using FluentValidation;
using FluentValidation.Results;


namespace biblioteca_kuantum.BusinessLayer.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateDocument> _createDocumentValidator;
        private readonly IValidator<UpdateDocument> _updateDocumentValidator;
        public DocumentService(IDocumentRepository documentRepository, IMapper mapper,IValidator<CreateDocument> createDocumentValidator , IValidator<UpdateDocument> updateDocumentValidator)
        {
            _documentRepository = documentRepository;
            _createDocumentValidator = createDocumentValidator;
            _mapper = mapper;
            _updateDocumentValidator = updateDocumentValidator;
        }

        public async Task<DocumentResponse?> CreateDocumentAsync(CreateDocument document, CancellationToken ct)
        {
            
            await  _createDocumentValidator.ValidateAndThrowAsync(document, ct);
     

            Document documentModel = _mapper.Map<Document>(document);

            Document? createdDocument = await _documentRepository.CreateDocument(documentModel, ct);

            return _mapper.Map<DocumentResponse>(createdDocument);

        }

        public async Task<bool> DeleteDocumentAsync(int id, CancellationToken ct)
        {
            return await _documentRepository.DeleteDocument(id, ct);
        }


        public async Task<PagedResult<DocumentResponse>> FindDocumentAsync(int? id, string? serialCode, string? publicationCode, string? searchText, int pageNumber, int pageSize, CancellationToken ct)
        {
            PagedResult<Document> modelPagedResult = await _documentRepository.FindDocument(id,serialCode, publicationCode, searchText, pageNumber, pageSize, ct);

            return _mapper.Map<PagedResult<DocumentResponse>>(modelPagedResult);
        }

        public async Task<DocumentResponse?> GetDocumentByIdAsync(int id, CancellationToken ct)
        {
            Document? documentModel = await _documentRepository.GetDocumentById(id, ct);

            return _mapper.Map<DocumentResponse>(documentModel);
        }

        public async Task<DocumentResponse?> UpdateDocumentAsync(int id, UpdateDocument document, CancellationToken ct = default)
        {
          await _updateDocumentValidator.ValidateAndThrowAsync(document, ct);

            Document updateDocument = _mapper.Map<Document>(document);

            var documentModel = await _documentRepository.UpdateDocument(id, updateDocument, ct);

            if (documentModel == null)
            {
                return null;

            }

            return _mapper.Map<DocumentResponse>(documentModel);

        }
    }
}
