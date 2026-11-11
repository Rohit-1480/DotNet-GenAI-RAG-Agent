using GenAI.Application.Interfaces;
using GenAI.Domain.Entities;
using GenAI.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
namespace GenAI.Infrastructure.Storage
{
    public class SqliteDocumentChunkStore : IDocumentChunkStore
    {
        private readonly GenAIDbContext _context;

        public SqliteDocumentChunkStore(GenAIDbContext context)
        {
            _context = context;
        }

        //public async Task AddChunksAsync(
        //    IReadOnlyList<DocumentChunk> chunks)
        //{
        //    if (chunks == null || chunks.Count == 0)
        //        return;

        //    var source = chunks[0].Source;

        //    await using var transaction =
        //        await _context.Database.BeginTransactionAsync();

        //    try
        //    {
        //        // Remove existing chunks belonging to the same source
        //        await _context.DocumentChunks
        //            .Where(x => EF.Functions.Collate(
        //                x.Source, "NOCASE") == source)
        //            .ExecuteDeleteAsync();

        //        // Add the latest document chunks
        //        await _context.DocumentChunks.AddRangeAsync(chunks);

        //        // Save changes
        //        await _context.SaveChangesAsync();

        //        // Commit transaction
        //        await transaction.CommitAsync();
        //    }
        //    catch
        //    {
        //        // Roll back if any operation fails
        //        await transaction.RollbackAsync();
        //        throw;
        //    }
        //    //        if (chunks == null || chunks.Count == 0)
        //    //            return;

        //    //        var source = chunks[0].Source;
        //    //        await _context.DocumentChunks
        //    //    .Where(x => EF.Functions.Collate(
        //    //        x.Source, "NOCASE") == source)
        //    //    .ExecuteDeleteAsync();

        //    //        await _context.DocumentChunks.AddRangeAsync(chunks);

        //    //        await _context.SaveChangesAsync();
        //    //        // Remove existing chunks belonging to the same source
        //    //        //var existingChunks = await _context.DocumentChunks
        //    //        //    .Where(x => x.Source.ToLower() == source.ToLower())
        //    //        //    .ToListAsync();

        //    //        var existingChunks = await _context.DocumentChunks
        //    //.Where(x => EF.Functions.Collate(
        //    //    x.Source, "NOCASE") == source)
        //    //.ToListAsync();

        //    //        //_context.DocumentChunks.RemoveRange(existingChunks);
        //    //        await _context.DocumentChunks
        //    //.Where(x => EF.Functions.Collate(
        //    //    x.Source, "NOCASE") == source)
        //    //.ExecuteDeleteAsync();



        //    //        // Add the latest document chunks
        //    //        await _context.DocumentChunks.AddRangeAsync(chunks);

        //    //        await _context.SaveChangesAsync();
        //}
        //    public async Task AddChunksAsync(
        //IReadOnlyList<DocumentChunk> chunks)
        //    {
        //        if (chunks == null || chunks.Count == 0)
        //            return;

        //        var source = chunks[0].Source;

        //        await using var transaction =
        //            await _context.Database.BeginTransactionAsync();

        //        try
        //        {
        //            // Find existing parent document
        //            var document = await _context.Documents
        //                .FirstOrDefaultAsync(d =>
        //                    EF.Functions.Collate(d.FileName, "NOCASE") == source);

        //            // Create parent document if it does not exist
        //            if (document == null)
        //            {
        //                document = new Document
        //                {
        //                    Id = Guid.NewGuid(),
        //                    FileName = source,
        //                    Category = "Uncategorized",
        //                    ContentType = "text/plain",
        //                    UploadedAt = DateTime.UtcNow
        //                };

        //                await _context.Documents.AddAsync(document);
        //                await _context.SaveChangesAsync();
        //            }

        //            // Remove existing chunks belonging to this source
        //            await _context.DocumentChunks
        //                .Where(x => EF.Functions.Collate(
        //                    x.Source, "NOCASE") == source)
        //                .ExecuteDeleteAsync();

        //            // Assign parent DocumentId to every new chunk
        //            //foreach (var chunk in chunks)
        //            //{
        //            //    chunk.DocumentId = document.Id;
        //            //}
        //            foreach (var chunk in chunks)
        //            {
        //                chunk.DocumentId = document.Id;
        //                chunk.Document = document;
        //            }

        //            // Insert new chunks
        //            await _context.DocumentChunks.AddRangeAsync(chunks);
        //            Console.WriteLine(
        //$"Document state: {_context.Entry(document).State}");

        //            Console.WriteLine(
        //                $"Document ID: {document.Id}");

        //            Console.WriteLine(
        //                $"Chunk Document ID: {chunks[0].DocumentId}");
        //            var trackedState = _context.Entry(document).State;
        //            await _context.SaveChangesAsync();

        //            await transaction.CommitAsync();
        //        }
        //        catch
        //        {
        //            await transaction.RollbackAsync();
        //            throw;
        //        }
        //    }

        public async Task AddChunksAsync(
            IReadOnlyList<DocumentChunk> chunks)
        {
            if (chunks == null || chunks.Count == 0)
                return;

            var source = chunks[0].Source;

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                // Find existing document
                var document = await _context.Documents
                    .FirstOrDefaultAsync(d =>
                        EF.Functions.Collate(d.FileName, "NOCASE") == source);

                // Create document if it does not exist
                if (document == null)
                {
                    document = new Document
                    {
                        Id = Guid.NewGuid(),
                        FileName = source,
                        Category = "Uncategorized",
                        ContentType = "text/plain",
                        UploadedAt = DateTime.UtcNow
                    };

                    _context.Documents.Add(document);
                }

                // Remove old chunks using EF Core tracking
                var existingChunks = await _context.DocumentChunks
                    .Where(x =>
                        EF.Functions.Collate(x.Source, "NOCASE") == source)
                    .ToListAsync();

                _context.DocumentChunks.RemoveRange(existingChunks);

                // Assign parent document to new chunks
                foreach (var chunk in chunks)
                {
                    chunk.DocumentId = document.Id;
                    chunk.Document = document;
                }

                // Add new chunks
                _context.DocumentChunks.AddRange(chunks);
                var parentExists = await _context.Documents
    .AnyAsync(d => d.Id == document.Id);
                var currentDocumentState = _context.Entry(document).State;
                var sql = _context.Documents
    .Where(d => d.Id == document.Id)
    .ToQueryString();
                var documentIdText = document.Id.ToString();

                var connection = _context.Database.GetDbConnection();

                await using var command = connection.CreateCommand();

                command.Transaction = _context.Database
                    .CurrentTransaction?.GetDbTransaction();

                command.CommandText =
                    "SELECT COUNT(*) FROM Documents WHERE Id = $id";

                var parameter = command.CreateParameter();
                parameter.ParameterName = "$id";
                parameter.Value = documentIdText;

                command.Parameters.Add(parameter);

                var rawParentCount = Convert.ToInt32(
                    await command.ExecuteScalarAsync());
                // Save parent and chunks together
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IReadOnlyList<DocumentChunk>> GetAllChunksAsync()
        {
            return await _context.DocumentChunks
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
