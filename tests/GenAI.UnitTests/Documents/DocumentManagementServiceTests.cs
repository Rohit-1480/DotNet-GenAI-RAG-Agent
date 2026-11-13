using GenAI.Domain.Entities;
using GenAI.Infrastructure.Documents;
using GenAI.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GenAI.UnitTests.Documents
{
    public class DocumentManagementServiceTests
    {
        [Fact]
        public async Task GetAllDocuments_ReturnsAllDocuments()
        {
            //// Arrange
            //await using var connection =
            //    new SqliteConnection("Data Source=:memory:");

            //await connection.OpenAsync();

            //var options = new DbContextOptionsBuilder<GenAIDbContext>()
            //    .UseSqlite(connection)
            //    .Options;

            //await using var context = new GenAIDbContext(options);

            //await context.Database.EnsureCreatedAsync();

            // Arrange
            var (connection, context, service) = await CreateServiceAsync();

            await using var connectionToDispose = connection;
            await using var contextToDispose = context;

            var document1 = new Document
            {
                FileName = "Forex_Guide.txt",
                Category = "Forex",
                ContentType = "text/plain",
                UploadedAt = DateTime.UtcNow
            };

            var document2 = new Document
            {
                FileName = "Banking_Guide.txt",
                Category = "Banking",
                ContentType = "text/plain",
                UploadedAt = DateTime.UtcNow
            };

            context.Documents.AddRange(document1, document2);
            await context.SaveChangesAsync();

           // var service = new DocumentManagementService(context);

            // Act
            var result = await service.GetAllDocumentsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, d => d.FileName == "Forex_Guide.txt");
            Assert.Contains(result, d => d.FileName == "Banking_Guide.txt");
        }
        [Fact]
        public async Task GetDocumentByIdAsync_ExistingId_ReturnsDocument()
        {
            //// Arrange
            //await using var connection =
            //    new SqliteConnection("Data Source=:memory:");

            //await connection.OpenAsync();

            //var options = new DbContextOptionsBuilder<GenAIDbContext>()
            //    .UseSqlite(connection)
            //    .Options;

            //await using var context = new GenAIDbContext(options);

            //await context.Database.EnsureCreatedAsync();
            var (connection, context, service) = await CreateServiceAsync();

            await using var connectionToDispose = connection;
            await using var contextToDispose = context;
            var document = new Document
            {
                FileName = "Forex_Guide.txt",
                Category = "Forex",
                ContentType = "text/plain",
                UploadedAt = DateTime.UtcNow
            };

            context.Documents.Add(document);
            await context.SaveChangesAsync();

            //var service = new DocumentManagementService(context);

            // Act
            var result = await service.GetDocumentByIdAsync(document.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(document.Id, result.Id);
            Assert.Equal("Forex_Guide.txt", result.FileName);
            Assert.Equal("Forex", result.Category);
        }


        [Fact]
        public async Task GetDocumentByIdAsync_UnknownId_ReturnsNull()
        {
            //// Arrange
            //await using var connection =
            //    new SqliteConnection("Data Source=:memory:");

            //await connection.OpenAsync();

            //var options = new DbContextOptionsBuilder<GenAIDbContext>()
            //    .UseSqlite(connection)
            //    .Options;

            //await using var context = new GenAIDbContext(options);

            //await context.Database.EnsureCreatedAsync();

            //var service = new DocumentManagementService(context);
            var (connection, context, service) = await CreateServiceAsync();

            await using var connectionToDispose = connection;
            await using var contextToDispose = context;
            var unknownId = Guid.NewGuid();

            // Act
            var result = await service.GetDocumentByIdAsync(unknownId);

            // Assert
            Assert.Null(result);
        }


        [Fact]
        public async Task DeleteDocumentAsync_ExistingId_ReturnsTrue()
        {
            //// Arrange
            //await using var connection =
            //    new SqliteConnection("Data Source=:memory:");

            //await connection.OpenAsync();

            //var options = new DbContextOptionsBuilder<GenAIDbContext>()
            //    .UseSqlite(connection)
            //    .Options;

            //await using var context = new GenAIDbContext(options);

            //await context.Database.EnsureCreatedAsync();
            var (connection, context, service) = await CreateServiceAsync();

            await using var connectionToDispose = connection;
            await using var contextToDispose = context;
            var document = new Document
            {
                FileName = "Delete_Test.txt",
                Category = "Testing",
                ContentType = "text/plain",
                UploadedAt = DateTime.UtcNow
            };

            context.Documents.Add(document);
            await context.SaveChangesAsync();

           // var service = new DocumentManagementService(context);

            // Act
            var result = await service.DeleteDocumentByIdAsync(document.Id);

            // Assert
            Assert.True(result);

            var deletedDocument = await context.Documents
                .FirstOrDefaultAsync(d => d.Id == document.Id);

            Assert.Null(deletedDocument);
        }

        [Fact]
        public async Task DeleteDocumentAsync_DeletesAssociatedChunks()
        {
            //// Arrange
            //await using var connection =
            //    new SqliteConnection("Data Source=:memory:");

            //await connection.OpenAsync();

            //var options = new DbContextOptionsBuilder<GenAIDbContext>()
            //    .UseSqlite(connection)
            //    .Options;

            //await using var context = new GenAIDbContext(options);

            //await context.Database.EnsureCreatedAsync();
            var (connection, context, service) = await CreateServiceAsync();

            await using var connectionToDispose = connection;
            await using var contextToDispose = context;
            var document = new Document
            {
                FileName = "Cascade_Test.txt",
                Category = "Testing",
                ContentType = "text/plain",
                UploadedAt = DateTime.UtcNow
            };

            var chunk = new DocumentChunk
            {
                Text = "This is a test chunk.",
                Source = document.FileName,
                Embedding = new float[] { 0.1f, 0.2f, 0.3f },
                Document = document
            };

            document.Chunks.Add(chunk);

            context.Documents.Add(document);
            await context.SaveChangesAsync();

           // var service = new DocumentManagementService(context);

            // Confirm chunk exists before deletion
            Assert.Equal(1, await context.DocumentChunks.CountAsync());

            // Act
            var result = await service.DeleteDocumentByIdAsync(document.Id);

            // Assert
            Assert.True(result);

            var remainingChunks = await context.DocumentChunks.CountAsync();

            Assert.Equal(0, remainingChunks);
        }

        [Fact]
        public async Task DeleteDocumentAsync_UnknownId_ReturnsFalse()
        {
            //// Arrange
            //await using var connection =
            //    new SqliteConnection("Data Source=:memory:");

            //await connection.OpenAsync();

            //var options = new DbContextOptionsBuilder<GenAIDbContext>()
            //    .UseSqlite(connection)
            //    .Options;

            //await using var context = new GenAIDbContext(options);

            //await context.Database.EnsureCreatedAsync();

            //var service = new DocumentManagementService(context);
            var (connection, context, service) = await CreateServiceAsync();

            await using var connectionToDispose = connection;
            await using var contextToDispose = context;
            var unknownId = Guid.NewGuid();

            // Act
            var result = await service.DeleteDocumentByIdAsync(unknownId);

            // Assert
            Assert.False(result);
        }
        ///
        private static async Task<(SqliteConnection Connection,GenAIDbContext Context,DocumentManagementService Service)> CreateServiceAsync()
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();

            var options = new DbContextOptionsBuilder<GenAIDbContext>()
                .UseSqlite(connection)
                .Options;

            var context = new GenAIDbContext(options);

            await context.Database.EnsureCreatedAsync();

            var service = new DocumentManagementService(context);

            return (connection, context, service);
        }
    }
}
