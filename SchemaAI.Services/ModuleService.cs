using CommonEntities;
using SchemaAI.DALContracts;
using SchemaAI.Entities;
using SchemaAI.Persistence;
using SchemaAI.ServiceContract;

namespace SchemaAI.Services
{
    public sealed class ModuleService : IModuleService
    {
        private readonly IModuleDal _dal;
        private readonly SchemaAIDbContext _db;
        private readonly ICurrentUser _currentUser;

        public ModuleService(IModuleDal dal, SchemaAIDbContext db, ICurrentUser currentUser)
        {
            _dal = dal;
            _db = db;
            _currentUser = currentUser;
        }

        public async Task<List<Module>> GetAllAsync()
                        => await _dal.GetAllAsync();
        public async Task<Module?> GetAsync(Guid moduleGuid)
            => await _dal.GetByGuidAsync(moduleGuid);

        public async Task<List<Module>> GetByApplicationAsync(Guid applicationGuid)
            => await _dal.GetByApplicationGuidAsync(applicationGuid);

        public async Task<Module> CreateAsync(Module module)
        {
            module.CreateSetupConfiguration(_currentUser);
            await _dal.AddAsync(module);
            await _db.SaveChangesAsync();
            return module;
        }

        public async Task UpdateAsync(Module module)
        {
            module.CreateSetupConfiguration(_currentUser);
            await _dal.UpdateAsync(module);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid moduleGuid)
        {
            var entity = await _dal.GetByGuidAsync(moduleGuid);
            if (entity == null) return;

            await _dal.SoftDeleteAsync(entity);
            await _db.SaveChangesAsync();
        }

        #region Paging

        public async Task<PagedResult<Module>> GetPagedAsync(
        int pageNumber,
        int pageSize)
        {
            return await _dal.GetPagedAsync(pageNumber, pageSize);
        }

        public async Task<List<Module>> GetScrollAsync(
        int lastId,
        int batchSize)
        {
            return await _dal.GetScrollAsync(lastId, batchSize);
        }

        #endregion
    }
}
