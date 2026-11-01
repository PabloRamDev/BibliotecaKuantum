
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace biblioteca_kuantum.DataAccessLayer.Models
{
    public class DocumentPageIndex
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Document")]
        public int DocumentId { get; set; }
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public int Page { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public virtual Document? Document { get; set; }
    }
}
