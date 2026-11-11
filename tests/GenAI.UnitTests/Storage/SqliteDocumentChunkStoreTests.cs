using GenAI.Domain.Entities;
using GenAI.Infrastructure.Persistence;
using GenAI.Infrastructure.Storage;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
namespace GenAI.UnitTests.Storage
{
    public class SqliteDocumentChunkStoreTests
    {
        private static async Task<(SqliteConnection Connection,
            GenAIDbContext Context,
            SqliteDocumentChunkStore Store)> CreateStoreAsync()
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();

            var options = new DbContextOptionsBuilder<GenAIDbContext>()
                .UseSqlite(connection)
                .Options;

            var context = new GenAIDbContext(options);

            await context.Database.EnsureCreatedAsync();

            var store = new SqliteDocumentChunkStore(context);

            return (connection, context, store);
        }
        [Fact]
        public async Task AddChunksAsync_ShouldStoreDocumentChunks()
        {
            // Arrange
            var (connection, context, store) = await CreateStoreAsync();

            await using var connectionToDispose = connection;
            await using var contextToDispose = context;

            var chunks = new List<DocumentChunk>
    {
        new()
        {
            Text = "Forex is the foreign exchange market.",
            Source = "Forex_Guide.txt",
            Embedding = new float[] { 0.1f, 0.2f, 0.3f }
        },
        new()
        {
            Text = "Currency pairs are traded in forex.",
            Source = "Forex_Guide.txt",
            Embedding = new float[] { 0.4f, 0.5f, 0.6f }
        }
    };

            // Act
            await store.AddChunksAsync(chunks);

            var result = await store.GetAllChunksAsync();

            // Assert
            Assert.Equal(2, result.Count);

            Assert.All(result, chunk =>
                Assert.Equal("Forex_Guide.txt", chunk.Source));
            var forexChunk = result.Single(chunk =>
    chunk.Text == "Forex is the foreign exchange market.");

            Assert.Equal(
                new float[] { 0.1f, 0.2f, 0.3f },
                forexChunk.Embedding);
            //Assert.Equal(
            //    new float[] { 0.1f, 0.2f, 0.3f },
            //    result[0].Embedding);
        }

        [Fact]
        public async Task AddChunksAsync_ShouldReplaceExistingChunks_WhenSameSourceIsAddedAgain()
        {
            // Arrange
            var (connection, context, store) = await CreateStoreAsync();

            await using var connectionToDispose = connection;
            await using var contextToDispose = context;

            var firstIngestion = new List<DocumentChunk>
    {
        new()
        {
            Text = "Old Forex content",
            Source = "Forex_Guide.txt",
            Embedding = new float[] { 0.1f, 0.2f }
        }
    };

            var secondIngestion = new List<DocumentChunk>
    {
        new()
        {
            Text = "Updated Forex content",
            Source = "Forex_Guide.txt",
            Embedding = new float[] { 0.3f, 0.4f }
        },
        new()
        {
            Text = "Additional Forex content",
            Source = "Forex_Guide.txt",
            Embedding = new float[] { 0.5f, 0.6f }
        }
    };

            // Act
            await store.AddChunksAsync(firstIngestion);
            await store.AddChunksAsync(secondIngestion);

            var result = await store.GetAllChunksAsync();

            // Assert
            Assert.Equal(2, result.Count);

            Assert.DoesNotContain(result, chunk =>
                chunk.Text == "Old Forex content");

            Assert.Contains(result, chunk =>
                chunk.Text == "Updated Forex content");

            Assert.Contains(result, chunk =>
                chunk.Text == "Additional Forex content");
        }

        [Fact]
        public async Task AddChunksAsync_ShouldPreserveOtherDocuments_WhenSameSourceIsReingested()
        {
            // Arrange
            var (connection, context, store) = await CreateStoreAsync();

            await using var connectionToDispose = connection;
            await using var contextToDispose = context;

            var forexChunks = new List<DocumentChunk>
    {
        new()
        {
            Text = "Original Forex content",
            Source = "Forex_Guide.txt",
            Embedding = new float[] { 0.1f, 0.2f }
        }
    };

            var bankingChunks = new List<DocumentChunk>
    {
        new()
        {
            Text = "Banking information",
            Source = "Banking_Guide.txt",
            Embedding = new float[] { 0.3f, 0.4f }
        }
    };

            var updatedForexChunks = new List<DocumentChunk>
    {
        new()
        {
            Text = "Updated Forex content",
            Source = "Forex_Guide.txt",
            Embedding = new float[] { 0.5f, 0.6f }
        }
    };

            // Act
            await store.AddChunksAsync(forexChunks);
            await store.AddChunksAsync(bankingChunks);
            await store.AddChunksAsync(updatedForexChunks);

            var result = await store.GetAllChunksAsync();

            // Assert
            Assert.Equal(2, result.Count);

            Assert.Contains(result, chunk =>
                chunk.Source == "Forex_Guide.txt" &&
                chunk.Text == "Updated Forex content");

            Assert.Contains(result, chunk =>
                chunk.Source == "Banking_Guide.txt" &&
                chunk.Text == "Banking information");

            Assert.DoesNotContain(result, chunk =>
                chunk.Text == "Original Forex content");
        }


        [Fact]
        public async Task AddChunksAsync_ShouldNotChangeExistingData_WhenEmptyListIsAdded()
        {
            // Arrange
            var (connection, context, store) = await CreateStoreAsync();

            await using var connectionToDispose = connection;
            await using var contextToDispose = context;

            var existingChunks = new List<DocumentChunk>
    {
        new()
        {
            Text = "Forex information",
            Source = "Forex_Guide.txt",
            Embedding = new float[] { 0.1f, 0.2f }
        }
    };

            // Act
            await store.AddChunksAsync(existingChunks);
            await store.AddChunksAsync(new List<DocumentChunk>());

            var result = await store.GetAllChunksAsync();

            // Assert
            Assert.Single(result);

            Assert.Equal("Forex_Guide.txt", result[0].Source);
            Assert.Equal("Forex information", result[0].Text);
        }


        [Fact]
        public async Task AddChunksAsync_ShouldReplaceExistingChunks_WhenSourceHasDifferentCasing()
        {
            // Arrange
            var (connection, context, store) = await CreateStoreAsync();

            await using var connectionToDispose = connection;
            await using var contextToDispose = context;

            var originalChunks = new List<DocumentChunk>
    {
        new()
        {
            Text = "Original Forex content",
            Source = "Forex_Guide.txt",
            Embedding = new float[] { 0.1f, 0.2f }
        }
    };

            var updatedChunks = new List<DocumentChunk>
    {
        new()
        {
            Text = "Updated Forex content",
            Source = "forex_guide.txt",
            Embedding = new float[] { 0.3f, 0.4f }
        }
    };

            // Act
            await store.AddChunksAsync(originalChunks);
            await store.AddChunksAsync(updatedChunks);

            var result = await store.GetAllChunksAsync();

            // Assert
            Assert.Single(result);

            Assert.Equal("Updated Forex content", result[0].Text);
            Assert.Equal("forex_guide.txt", result[0].Source);
        }
        //    [Fact]
        //    public async Task AddChunksAsync_ShouldRollbackDeletion_WhenInsertionFails()
        //    {
        //        // Arrange
        //        var (connection, context, store) = await CreateStoreAsync();

        //        await using var connectionToDispose = connection;
        //        await using var contextToDispose = context;

        //        var originalChunk = new DocumentChunk
        //        {
        //            Id = Guid.NewGuid(),
        //            Text = "Original Forex content",
        //            Source = "Forex_Guide.txt",
        //            Embedding = new float[] { 0.1f, 0.2f }
        //        };

        //        await store.AddChunksAsync(new List<DocumentChunk>
        //{
        //    originalChunk
        //});

        //        // Attempt to insert a chunk with the same primary key
        //        var invalidReplacement = new DocumentChunk
        //        {
        //            Id = originalChunk.Id,
        //            Text = "Invalid replacement content",
        //            Source = "Forex_Guide.txt",
        //            Embedding = new float[] { 0.3f, 0.4f }
        //        };

        //        // Act
        //        await Assert.ThrowsAsync<DbUpdateException>(async () =>
        //        {
        //            await store.AddChunksAsync(new List<DocumentChunk>
        //    {
        //        invalidReplacement
        //    });
        //        });

        //        var result = await store.GetAllChunksAsync();

        //        // Assert
        //        Assert.Single(result);

        //        Assert.Equal(originalChunk.Id, result[0].Id);
        //        Assert.Equal("Original Forex content", result[0].Text);
        //        Assert.Equal("Forex_Guide.txt", result[0].Source);
        //    }
        [Fact]
        public async Task AddChunksAsync_ShouldRollbackDeletion_WhenInsertionFails()
        {
            // Arrange
            var (connection, context, store) = await CreateStoreAsync();

            await using var connectionToDispose = connection;
            await using var contextToDispose = context;

            var originalChunk = new DocumentChunk
            {
                Id = Guid.NewGuid(),
                Text = "Original Forex content",
                Source = "Forex_Guide.txt",
                Embedding = new float[] { 0.1f, 0.2f }
            };

            await store.AddChunksAsync(new List<DocumentChunk>
    {
        originalChunk
    });

            // Clear tracked entities
            context.ChangeTracker.Clear();

            // Create an invalid replacement (Text violates NOT NULL constraint)
            var invalidReplacement = new DocumentChunk
            {
                Id = Guid.NewGuid(),
                Text = null!,
                Source = "Forex_Guide.txt",
                Embedding = new float[] { 0.3f, 0.4f }
            };

            // Act
            await Assert.ThrowsAsync<DbUpdateException>(async () =>
            {
                await store.AddChunksAsync(new List<DocumentChunk>
        {
            invalidReplacement
        });
            });

            var result = await store.GetAllChunksAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal(originalChunk.Id, result[0].Id);
            Assert.Equal("Original Forex content", result[0].Text);
            Assert.Equal("Forex_Guide.txt", result[0].Source);
        }
    }
}
