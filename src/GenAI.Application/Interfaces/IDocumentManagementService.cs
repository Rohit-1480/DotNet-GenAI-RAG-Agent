using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenAI.Application.DTO;
using GenAI.Domain.Entities;
namespace GenAI.Application.Interfaces
{
    public interface IDocumentManagementService
    {
        Task<IReadOnlyList<DocumentResponseDto>> GetAllDocumentsAsync();
        Task<DocumentResponseDto?> GetDocumentByIdAsync(Guid id);
        Task<bool> DeleteDocumentByIdAsync(Guid id);
    }
}
