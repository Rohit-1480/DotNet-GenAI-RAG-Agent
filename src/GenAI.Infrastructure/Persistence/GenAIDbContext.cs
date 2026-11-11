using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
namespace GenAI.Infrastructure.Persistence
{
    public class GenAIDbContext : DbContext
    {
        public GenAIDbContext(DbContextOptions<GenAIDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Index for faster source-based document lookup
            //        modelBuilder.Entity<DocumentChunk>()
            //.HasIndex(x => x.Source)
            //.HasDatabaseName("IX_DocumentChunks_Source");
            //modelBuilder.Entity<DocumentChunk>()
            //.HasIndex(x => x.Source)
            //.HasDatabaseName("IX_DocumentChunks_Source")
            //.UseCollation("NOCASE");
            // Configure case-insensitive collation for Source
            // Configure Document to DocumentChunk relationship
            modelBuilder.Entity<Document>()
                .HasMany(d => d.Chunks)
                .WithOne(c => c.Document)
                .HasForeignKey(c => c.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<DocumentChunk>()
                .Property(x => x.Source)
                .UseCollation("NOCASE");
            modelBuilder.Entity<Document>()
    .Property(d => d.Id)
    .UseCollation("NOCASE");
            // Index for faster source-based document lookup
            modelBuilder.Entity<DocumentChunk>()
                .HasIndex(x => x.Source)
                .HasDatabaseName("IX_DocumentChunks_Source");
            //modelBuilder.Entity<DocumentChunk>()
            //    .Property(x => x.Embedding)
            //    .HasConversion(
            //        v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
            //        v => System.Text.Json.JsonSerializer.Deserialize<float[]>(v, (System.Text.Json.JsonSerializerOptions?)null)
            //             ?? Array.Empty<float>());
            var embeddingProperty = modelBuilder.Entity<DocumentChunk>()
    .Property(x => x.Embedding);

            embeddingProperty.HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<float[]>(v, (System.Text.Json.JsonSerializerOptions?)null)
                     ?? Array.Empty<float>());

            embeddingProperty.Metadata.SetValueComparer(
                new ValueComparer<float[]>(
                    (a, b) => a != null && b != null && a.SequenceEqual(b),
                    v => v.Aggregate(0, (hash, item) => HashCode.Combine(hash, item.GetHashCode())),
                    v => v.ToArray()));
        }
        public DbSet<Document> Documents => Set<Document>();
        public DbSet<DocumentChunk> DocumentChunks => Set<DocumentChunk>();
    }
}
