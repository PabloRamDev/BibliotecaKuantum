using AutoMapper;
using biblioteca_kuantum.BusinessLayer.DTO;
using biblioteca_kuantum.DataAccessLayer.Common;
using biblioteca_kuantum.DataAccessLayer.Models;


namespace biblioteca_kuantum.BusinessLayer.Mappers
{
    public class DocumentMappingProfiles : Profile
    {
        public DocumentMappingProfiles()
        {
            CreateMap<Document, DocumentResponse>().ReverseMap();
            CreateMap<Document, CreateDocument>().ReverseMap();
            CreateMap<DocumentPageIndex, DocumentPageIndexResponse>().ReverseMap();
            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
        }
    }
}
