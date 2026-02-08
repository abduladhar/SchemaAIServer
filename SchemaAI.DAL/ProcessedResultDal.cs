using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SchemaAI.DALContracts;
using SchemaAI.Entities;
using SchemaAI.Persistence;

namespace SchemaAI.DAL
{
    public sealed class ProcessedResultDal
        : BaseDal<ProcessedResult>, IProcessedResultDal
    {
        public ProcessedResultDal(SchemaAIDbContext db)
            : base(db)
        {
        }

        public async Task<ProcessedResult?> GetByProcessingRequestGuidAsync(Guid processingRequestGuid)
        {
            return await _db.ProcessedResults
                .FirstOrDefaultAsync(x => x.ProcessingRequestGuid == processingRequestGuid);
        }

        public async Task<ProcessedResult?> GetByProcessedGuidAsync(Guid processedRequestGuid)
        {
            return await _db.ProcessedResults
                .FirstOrDefaultAsync(x => x.ProcessedResultGuid == processedRequestGuid);
        }

        public async Task<ProcessedResult?> GetByReferanceNoAsync(string szReferanceNo)
        {
            return await _db.ProcessedResults
                           .IgnoreQueryFilters()   // 🔥 THIS bypasses IsDeleted + Tenant filters
                           .FirstOrDefaultAsync(x => x.ReferenceNumber == szReferanceNo);
           
        }


        public async Task<List<ProcessedResult>> GetAllByProcessingRequestGuidAsync(Guid processingRequestGuid)
        {
            return await _db.ProcessedResults
                .Where(x => x.ProcessingRequestGuid == processingRequestGuid)
                .ToListAsync();
        }
    }
}
