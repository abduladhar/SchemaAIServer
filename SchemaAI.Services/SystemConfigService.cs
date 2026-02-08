using CommonEntities;
using Microsoft.EntityFrameworkCore;
using SchemaAI.DAL;
using SchemaAI.DALContracts;
using SchemaAI.Entities;
using SchemaAI.Persistence;
using SchemaAI.ServiceContract;

namespace SchemaAI.Services
{
    public sealed class SystemConfigService : ISystemConfigService
    {
        private readonly ISystemConfigDal _dal;
        private readonly SchemaAIDbContext _db;
        private readonly ICurrentUser _currentUser;

        public SystemConfigService(ISystemConfigDal dal, SchemaAIDbContext db, ICurrentUser currentUser)
        {
            _dal = dal;
            _db = db;
            _currentUser = currentUser;
        }

        public async Task<List<SystemConfig>> GetAllAsync()
                        => await _dal.GetAllAsync();

       
        public async Task<SystemConfig?> GetAsync(Guid systemConfigGuid)
            => await _dal.GetByGuidAsync(systemConfigGuid);

        public async Task<SystemConfig?> GetAsync(string p_szApiKey)
          => await _dal.GetByApiKey(p_szApiKey);

        public async Task<SystemConfig> CreateAsync(SystemConfig awsConfig)
        {
            awsConfig.CreateSetupConfiguration(_currentUser);
            await _dal.AddAsync(awsConfig);
            await _db.SaveChangesAsync();
            return awsConfig;
        }

        public async Task UpdateAsync(SystemConfig awsConfig)
        {
            awsConfig.CreateSetupConfiguration(_currentUser);
            await _dal.UpdateAsync(awsConfig);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid aWSConfigGuid)
        {
            var entity = await _dal.GetByGuidAsync(aWSConfigGuid);
            if (entity == null) return;

            await _dal.SoftDeleteAsync(entity);
            await _db.SaveChangesAsync();
        }

        #region Paging

        public async Task<PagedResult<SystemConfig>> GetPagedAsync(
        int pageNumber,
        int pageSize)
        {
            return await _dal.GetPagedAsync(pageNumber, pageSize);
        }

        public async Task<List<SystemConfig>> GetScrollAsync(
        int lastId,
        int batchSize)
        {
            return await _dal.GetScrollAsync(lastId, batchSize);
        }

        #endregion
    }
}
