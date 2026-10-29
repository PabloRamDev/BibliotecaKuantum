
namespace biblioteca_kuantum.BusinessLayer.DTO
{
    public class DocumentPageIndexResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Page { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
