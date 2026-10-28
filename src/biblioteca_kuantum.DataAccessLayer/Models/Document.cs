
using System.ComponentModel.DataAnnotations;

namespace biblioteca_kuantum.DataAccessLayer.Models
{
    public class Document
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(100)]
        public string? Name { get; set; }
        [MaxLength(1000)]
        public string? Description { get; set; }
        public string? AuthorFullName { get; set; }
        [EmailAddress]
        [MaxLength(100)]
        public string? AuthorEmail { get; set; }
        [MaxLength(16)]
        public string SerialCode { get; set; } = string.Empty;
        [MaxLength(100)]
        public string PublicationCode { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public DateTimeOffset DeletedAt { get; set; }
    }
}
