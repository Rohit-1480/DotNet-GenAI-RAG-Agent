using GenAI.Application.Interfaces;
using GenAI.Domain.Entities;
using GenAI.Infrastructure.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAI.UnitTests
{
    public class SemanticSearchServiceTests
    {
        [Fact]
        public async Task SearchAsync_HigherScoreChunk_ReturnsFirst()
        {
            // Arrange
            var chunk1 = new DocumentChunk
            {
                Text = "ASP.NET Core dependency injection",
                Embedding = [1, 0]
            };

            var chunk2 = new DocumentChunk
            {
                Text = "Cooking recipes",
                Embedding = [0, 1]
            };

            var embeddingService = new FakeEmbeddingService();
            var chunkStore = new FakeDocumentChunkStore(
                [chunk1, chunk2]);
            var similarityService = new FakeSimilarityService();

            var service = new SemanticSearchService(
                embeddingService,
                chunkStore,
                similarityService);

            // Act
            var results = await service.SearchAsync(
                "How does dependency injection work?");

            // Assert
            Assert.Equal(2, results.Count);
            Assert.Equal(chunk1.Id, results[0].Chunk.Id);
            Assert.Equal(0.9, results[0].Score);
            Assert.Equal(chunk2.Id, results[1].Chunk.Id);
            Assert.Equal(0.2, results[1].Score);
        }
        [Fact]
        public async Task SearchAsync_TopKIsOne_ReturnsOnlyOneResult()
        {
            // Arrange
            var chunk1 = new DocumentChunk
            {
                Text = "ASP.NET Core dependency injection",
                Embedding = [1, 0]
            };

            var chunk2 = new DocumentChunk
            {
                Text = "Cooking recipes",
                Embedding = [0, 1]
            };

            var service = new SemanticSearchService(
                new FakeEmbeddingService(),
                new FakeDocumentChunkStore([chunk1, chunk2]),
                new FakeSimilarityService());

            // Act
            var results = await service.SearchAsync(
                "How does dependency injection work?",
                topK: 1);

            // Assert
            Assert.Single(results);
            Assert.Equal(chunk1.Id, results[0].Chunk.Id);
        }
        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task SearchAsync_EmptyQuery_ThrowsArgumentException(
    string query)
        {
            // Arrange
            var service = new SemanticSearchService(
                new FakeEmbeddingService(),
                new FakeDocumentChunkStore([]),
                new FakeSimilarityService());

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => service.SearchAsync(query));
        }
        [Fact]
        public async Task SearchAsync_WithForexSource_ReturnsOnlyForexChunks()
        {
            // Arrange
            var forexChunk = new DocumentChunk
            {
                Text = "Forex exchange rates",
                Source = "Forex_Guide.txt",
                Embedding = [1, 0]
            };

            var bankingChunk = new DocumentChunk
            {
                Text = "Banking services",
                Source = "Banking_Guide.txt",
                Embedding = [1, 0]
            };

            var service = new SemanticSearchService(
                new FakeEmbeddingService(),
                new FakeDocumentChunkStore([forexChunk, bankingChunk]),
                new FakeSimilarityService());

            // Act
            var results = await service.SearchAsync(
                "currency exchange",
                source: "Forex_Guide.txt");

            // Assert
            Assert.Single(results);
            Assert.Equal("Forex_Guide.txt", results[0].Chunk.Source);
        }

        [Fact]
        public async Task SearchAsync_WithBankingSource_ReturnsOnlyBankingChunks()
        {
            // Arrange
            var forexChunk = new DocumentChunk
            {
                Text = "Forex exchange rates",
                Source = "Forex_Guide.txt",
                Embedding = [1, 0]
            };

            var bankingChunk = new DocumentChunk
            {
                Text = "Banking services",
                Source = "Banking_Guide.txt",
                Embedding = [1, 0]
            };

            var service = new SemanticSearchService(
                new FakeEmbeddingService(),
                new FakeDocumentChunkStore([forexChunk, bankingChunk]),
                new FakeSimilarityService());

            // Act
            var results = await service.SearchAsync(
                "banking services",
                source: "Banking_Guide.txt");

            // Assert
            Assert.Single(results);
            Assert.Equal("Banking_Guide.txt", results[0].Chunk.Source);
        }


        [Fact]
        public async Task SearchAsync_SourceMatchingIsCaseInsensitive()
        {
            // Arrange
            var chunk = new DocumentChunk
            {
                Text = "Forex exchange rates",
                Source = "Forex_Guide.txt",
                Embedding = [1, 0]
            };

            var service = new SemanticSearchService(
                new FakeEmbeddingService(),
                new FakeDocumentChunkStore([chunk]),
                new FakeSimilarityService());

            // Act
            var results = await service.SearchAsync(
                "currency exchange",
                source: "forex_guide.TXT");

            // Assert
            Assert.Single(results);
        }


        [Fact]
        public async Task SearchAsync_WithUnknownSource_ReturnsEmptyResults()
        {
            // Arrange
            var chunk = new DocumentChunk
            {
                Text = "Forex exchange rates",
                Source = "Forex_Guide.txt",
                Embedding = [1, 0]
            };

            var service = new SemanticSearchService(
                new FakeEmbeddingService(),
                new FakeDocumentChunkStore([chunk]),
                new FakeSimilarityService());

            // Act
            var results = await service.SearchAsync(
                "currency exchange",
                source: "Unknown.txt");

            // Assert
            Assert.Empty(results);
        }


        [Fact]
        public async Task SearchAsync_WithNullSource_SearchesAllDocuments()
        {
            // Arrange
            var forexChunk = new DocumentChunk
            {
                Text = "Forex exchange rates",
                Source = "Forex_Guide.txt",
                Embedding = [1, 0]
            };

            var bankingChunk = new DocumentChunk
            {
                Text = "Banking services",
                Source = "Banking_Guide.txt",
                Embedding = [1, 0]
            };

            var service = new SemanticSearchService(
                new FakeEmbeddingService(),
                new FakeDocumentChunkStore([forexChunk, bankingChunk]),
                new FakeSimilarityService());

            // Act
            var results = await service.SearchAsync(
                "general search",
                source: null);

            // Assert
            Assert.Equal(2, results.Count);
        }
        private sealed class FakeEmbeddingService : IEmbeddingService
        {
            public Task<float[]> GetEmbeddingAsync(string text)
            {
                return Task.FromResult(new float[] { 1, 0 });
            }
        }
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task SearchAsync_InvalidTopK_ThrowsArgumentOutOfRangeException(
    int topK)
        {
            // Arrange
            var service = new SemanticSearchService(
                new FakeEmbeddingService(),
                new FakeDocumentChunkStore([]),
                new FakeSimilarityService());

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                () => service.SearchAsync("dependency injection", topK));
        }
        private sealed class FakeDocumentChunkStore : IDocumentChunkStore
        {
            private readonly IReadOnlyList<DocumentChunk> _chunks;

            public FakeDocumentChunkStore(
                IReadOnlyList<DocumentChunk> chunks)
            {
                _chunks = chunks;
            }

            public Task AddChunksAsync(
                IReadOnlyList<DocumentChunk> chunks)
            {
                throw new NotImplementedException();
            }

            public Task<IReadOnlyList<DocumentChunk>> GetAllChunksAsync()
            {
                return Task.FromResult(_chunks);
            }
        }

        private sealed class FakeSimilarityService : ISimilarityService
        {
            public double CalculateCosineSimilarity(
                float[] vectorA,
                float[] vectorB)
            {
                return vectorB[0] == 1 ? 0.9 : 0.2;
            }
        }
    }
}
