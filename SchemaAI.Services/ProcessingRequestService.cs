using CommonEntities;
using SchemaAI.DALContracts;
using SchemaAI.Entities;
using SchemaAI.Persistence;
using SchemaAI.ServiceContract;

namespace SchemaAI.Services
{
    public sealed class ProcessingRequestService : IProcessingRequestService
    {
        private readonly IProcessingRequestDal _dal;
        private readonly SchemaAIDbContext _db;
        private readonly ICurrentUser _currentUser;
        public ProcessingRequestService(
            IProcessingRequestDal dal,
            SchemaAIDbContext db,
            ICurrentUser currentUser)
        {
            _dal = dal;
            _db = db;
            _currentUser = currentUser;
        }

        public async Task<ProcessingRequest?> GetByReferenceAsync(string referenceNumber)
            => await _dal.GetByReferenceAsync(referenceNumber);

        public async Task<ProcessingRequest> CreateAsync(ProcessingRequest request)
        {
            try
            {
                await _dal.AddAsync(request);
                await _db.SaveChangesAsync();
                return request;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message);
            }
        }

        public async Task UpdateStatusAsync(Guid requestGuid, string status)
        {
            var entity = await _dal.GetByGuidAsync(requestGuid);
            if (entity == null) return;

            entity.Status = status.ToString();
            await _dal.UpdateAsync(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<List<ProcessingRequest>> GetAllAsync()
                     => await _dal.GetAllAsync();

        #region Paging

        public async Task<PagedResult<ProcessingRequest>> GetPagedAsync(
        int pageNumber,
        int pageSize)
        {
            return await _dal.GetPagedAsync(pageNumber, pageSize);
        }

        public async Task<List<ProcessingRequest>> GetScrollAsync(
        int lastId,
        int batchSize)
        {
            return await _dal.GetScrollAsync(lastId, batchSize);
        }

        #endregion
    }
}
