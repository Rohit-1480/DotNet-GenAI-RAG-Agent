using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAI.Application.DTO
{
    public record DocumentResponseDto(
        Guid Id,
        string FileName,
        string Category,
        string ContentType,
        DateTime UploadedAt
    );
}
