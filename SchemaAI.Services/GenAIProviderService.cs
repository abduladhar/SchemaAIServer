using CommonEntities;
using Microsoft.EntityFrameworkCore;
using SchemaAI.DAL;
using SchemaAI.DALContracts;
using SchemaAI.Entities;
using SchemaAI.Persistence;
using SchemaAI.ServiceContract;

namespace SchemaAI.Services
{
    public sealed class GenAIProviderService : IGenAIProviderService
    {
        private readonly IGenAIProviderDal _dal;
        private readonly SchemaAIDbContext _db;
        private readonly ICurrentUser _currentUser;
        public GenAIProviderService(IGenAIProviderDal dal, SchemaAIDbContext db, ICurrentUser currentUser)
        {
            _dal = dal;
            _db = db;
            _currentUser = currentUser;
        }

        public async Task<List<GenAIProvider>> GetAllAsync()
                        => await _dal.GetAllAsync();

       
        public async Task<GenAIProvider?> GetAsync(Guid genAIProviderGuid)
            => await _dal.GetByGuidAsync(genAIProviderGuid);

        public async Task<GenAIProvider> CreateAsync(GenAIProvider genAIProvider)
        {
            genAIProvider.CreateSetupConfiguration(_currentUser);
            await _dal.AddAsync(genAIProvider);
            await _db.SaveChangesAsync();
            return genAIProvider;
        }

        public async Task UpdateAsync(GenAIProvider genAIProvider)
        {
            genAIProvider.CreateSetupConfiguration(_currentUser);
            await _dal.UpdateAsync(genAIProvider);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid genAIProviderGuid)
        {
            var entity = await _dal.GetByGuidAsync(genAIProviderGuid);
            if (entity == null) return;

            await _dal.SoftDeleteAsync(entity);
            await _db.SaveChangesAsync();
        }

        #region Paging

        public async Task<PagedResult<GenAIProvider>> GetPagedAsync(
        int pageNumber,
        int pageSize)
        {
            return await _dal.GetPagedAsync(pageNumber, pageSize);
        }

        public async Task<List<GenAIProvider>> GetScrollAsync(
        int lastId,
        int batchSize)
        {
            return await _dal.GetScrollAsync(lastId, batchSize);
        }

        #endregion
    }
}
