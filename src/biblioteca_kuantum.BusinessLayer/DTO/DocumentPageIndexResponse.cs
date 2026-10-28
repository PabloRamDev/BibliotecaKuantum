using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace biblioteca_kuantum.BusinessLayer.DTO
{
    public class DocumentPageIndexResponse
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public string? Name { get; set; }
        public int Page { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
