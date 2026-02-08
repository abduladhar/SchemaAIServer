using Microsoft.EntityFrameworkCore;
using SchemaAI.DALContracts;
using SchemaAI.Entities;
using SchemaAI.Persistence;

namespace SchemaAI.DAL
{
    public sealed class UploadedFileDal
    : BaseDal<UploadedFile>, IUploadedFileDal
    {
        public UploadedFileDal(SchemaAIDbContext db) : base(db) { }

        public async Task<UploadedFile?> GetByReferenceAsync(string referenceNumber)
        {
            return await _db.UploadedFiles
                .FirstOrDefaultAsync(x => x.ReferenceNumber == referenceNumber);
        }

        public async Task<UploadedFile?> GetUploadedFilebyGuidAsync(Guid p_UploadedFileGuid)
        {
            return await _db.UploadedFiles
                .FirstOrDefaultAsync(x => x.UploadedFileGuid == p_UploadedFileGuid);
        }
    }
}
