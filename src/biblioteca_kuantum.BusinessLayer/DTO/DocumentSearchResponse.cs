using biblioteca_kuantum.DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace biblioteca_kuantum.BusinessLayer.DTO
{
    public class DocumentSearchResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? AuthorFullName { get; set; }
        public string? AuthorEmail { get; set; }
        public string SerialCode { get; set; } = string.Empty;
        public string PublicationCode { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
