using biblioteca_kuantum.DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace biblioteca_kuantum.BusinessLayer.DTO
{
    public class CreateDocument
    {
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
        public ICollection<CreateDocumentPageIndex>? DocumentPages { get; set; }
    }
}
