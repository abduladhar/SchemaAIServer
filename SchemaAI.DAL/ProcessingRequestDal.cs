using Microsoft.EntityFrameworkCore;
using SchemaAI.DALContracts;
using SchemaAI.Entities;
using SchemaAI.Persistence;

namespace SchemaAI.DAL
{
    public sealed class ProcessingRequestDal
    : BaseDal<ProcessingRequest>, IProcessingRequestDal
    {
        public ProcessingRequestDal(SchemaAIDbContext db) : base(db) { }

        public async Task<ProcessingRequest?> GetByReferenceAsync(string referenceNumber)
        {
            return await _db.ProcessingRequests
                .FirstOrDefaultAsync(x => x.ReferenceNumber == referenceNumber);
        }
    }
}
