using CommonEntities;
using SchemaAI.DALContracts;
using SchemaAI.Entities;
using SchemaAI.Persistence;
using SchemaAI.ServiceContract;

namespace SchemaAI.Services
{
    public sealed class ProcessedResultService : IProcessedResultService
    {
        private readonly IProcessedResultDal _dal;
        private readonly SchemaAIDbContext _db;
        private readonly ICurrentUser _currentUser;
        private readonly ISystemConfigService _systemConfigService;
        public ProcessedResultService(
            IProcessedResultDal dal,
            SchemaAIDbContext db,
            ICurrentUser currentUser,
            ISystemConfigService systemConfigService)
        {
            _dal = dal;
            _db = db;
            _currentUser = currentUser;
            _systemConfigService = systemConfigService;
        }

        public async Task<ProcessedResult?> GetByRequestAsync(Guid processingRequestGuid)
            => await _dal.GetByProcessingRequestGuidAsync(processingRequestGuid);

        public async Task<List<ProcessedResult>> GetAllAsync()
                       => await _dal.GetAllAsync();

        public async Task<ProcessedResult?> GetByProcessedGuidAsync(Guid processedRequestGuid)
           => await _dal.GetByProcessedGuidAsync(processedRequestGuid);

        public async Task<ProcessedResult> CreateAsync(ProcessedResult result)
        {
            await _dal.AddAsync(result);
            await _db.SaveChangesAsync();
            return result;
        }

        public async Task<ProcessedResult?> GetByReferenceNoAsync(
        string referenceNo,
        string apiKey)
        {
            var systemConfig = await _systemConfigService.GetAsync(apiKey);

            if (systemConfig == null || systemConfig.SystemConfigGuid == Guid.Empty)
            {
                throw new UnauthorizedAccessException("Invalid API key.");
            }

            return await _dal.GetByReferanceNoAsync(referenceNo);
        }

        #region Paging

        public async Task<PagedResult<ProcessedResult>> GetPagedAsync(
        int pageNumber,
        int pageSize)
        {
            return await _dal.GetPagedAsync(pageNumber, pageSize);
        }

        public async Task<List<ProcessedResult>> GetScrollAsync(
        int lastId,
        int batchSize)
        {
            return await _dal.GetScrollAsync(lastId, batchSize);
        }

        #endregion
    }
}
