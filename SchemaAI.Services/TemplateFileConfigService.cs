using CommonEntities;
using SchemaAI.DALContracts;
using SchemaAI.Entities;
using SchemaAI.Persistence;
using SchemaAI.ServiceContract;

namespace SchemaAI.Services
{
    public sealed class TemplateFileConfigService : ITemplateFileConfigService
    {
        private readonly ITemplateFileConfigDal _dal;
        private readonly SchemaAIDbContext _db;
        private readonly ICurrentUser _currentUser;

        public TemplateFileConfigService(
            ITemplateFileConfigDal dal,
            SchemaAIDbContext db,
            ICurrentUser currentUser)
        {
            _dal = dal;
            _db = db;
            _currentUser = currentUser;
        }

        public async Task<TemplateFileConfig?> GetByTemplateAsync(Guid templateGuid)
            => await _dal.GetByTemplateGuidAsync(templateGuid);

        public async Task<TemplateFileConfig> CreateOrUpdateAsync(TemplateFileConfig config)
        {
            var existing = await _dal.GetByTemplateGuidAsync(config.TemplateGuid);

            if (existing == null)
            {
                config.CreateSetupConfiguration(_currentUser);
                await _dal.AddAsync(config);
            }
            else
            {
                existing.CreateSetupConfiguration(_currentUser);
                existing.AllowedFileTypesCsv = config.AllowedFileTypesCsv;
                existing.MaxFileSizeInBytes = config.MaxFileSizeInBytes;
                await _dal.UpdateAsync(existing);
            }

            await _db.SaveChangesAsync();
            return config;
        }

        #region Paging

        public async Task<PagedResult<TemplateFileConfig>> GetPagedAsync(
        int pageNumber,
        int pageSize)
        {
            return await _dal.GetPagedAsync(pageNumber, pageSize);
        }

        public async Task<List<TemplateFileConfig>> GetScrollAsync(
        int lastId,
        int batchSize)
        {
            return await _dal.GetScrollAsync(lastId, batchSize);
        }

        #endregion
    }
}
