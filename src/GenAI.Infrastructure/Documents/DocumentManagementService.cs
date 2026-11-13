using GenAI.Application.DTO;
using GenAI.Application.Interfaces;
using GenAI.Domain.Entities;
using GenAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GenAI.Infrastructure.Documents
{
    public class DocumentManagementService: IDocumentManagementService
    {
        //private readonly GenAIDbContext _context;
        //public DocumentManagementService(GenAIDbContext dbContext)
        //{
        //    _context = dbContext;
        //}
        //public async Task<IReadOnlyList<Document>> GetAllDocumentsAsync()
        //{
        //    return await _context.Documents
        //        .AsNoTracking()
        //        .OrderByDescending(d => d.UploadedAt)
        //        .ToListAsync();
        //}
        private readonly GenAIDbContext _context;

        public DocumentManagementService(GenAIDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<DocumentResponseDto>> GetAllDocumentsAsync()
        {
            return await _context.Documents
                .AsNoTracking()
                .OrderByDescending(d => d.UploadedAt)
                .Select(d => new DocumentResponseDto(
                    d.Id,
                    d.FileName,
                    d.Category,
                    d.ContentType,
                    d.UploadedAt
                ))
                .ToListAsync();
        }
        public async Task<DocumentResponseDto?> GetDocumentByIdAsync(Guid id)
        {
            return await _context.Documents
                .AsNoTracking()
                .Where(d => d.Id == id)
                .Select(d => new DocumentResponseDto(
                    d.Id,
                    d.FileName,
                    d.Category,
                    d.ContentType,
                    d.UploadedAt
                ))
                .FirstOrDefaultAsync();
        }
        public async Task<bool> DeleteDocumentByIdAsync(Guid id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                return false;
            }
            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
            return true;
        }
    }
  
}
