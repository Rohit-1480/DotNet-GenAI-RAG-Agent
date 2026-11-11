using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAI.Domain.Entities
{
    public class Document
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string FileName { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string ContentType { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        // One document can contain multiple chunks
        public ICollection<DocumentChunk> Chunks { get; set; }
            = new List<DocumentChunk>();
    }
}
