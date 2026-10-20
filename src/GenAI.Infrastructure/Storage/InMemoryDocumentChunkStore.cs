using GenAI.Application.Interfaces;
using GenAI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAI.Infrastructure.Storage
{
    public class InMemoryDocumentChunkStore : IDocumentChunkStore
    {
        private readonly List<DocumentChunk> _chunks = new();
        public Task AddChunksAsync(IReadOnlyList<DocumentChunk> chunks)
        {
            _chunks.AddRange(chunks);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<DocumentChunk>> GetAllChunksAsync()
        {
            IReadOnlyList<DocumentChunk> result = _chunks.ToList();
            return Task.FromResult(result);
        }
    }
}
