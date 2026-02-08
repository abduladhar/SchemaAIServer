using CommonEntities;
using SchemaAI.DALContracts;
using SchemaAI.Entities;
using SchemaAI.Persistence;
using SchemaAI.ServiceContract;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchemaAI.Services
{
    public sealed class ApplicationService : IApplicationService
    {
        private readonly IApplicationDal _dal;
        private readonly SchemaAIDbContext _db;
        private readonly ICurrentUser _currentUser;

        public ApplicationService(IApplicationDal dal, SchemaAIDbContext db, ICurrentUser currentUser)
        {
            _dal = dal;
            _db = db;
            _currentUser = currentUser;
        }

        public async Task<Application?> GetAsync(Guid applicationGuid)
            => await _dal.GetByGuidAsync(applicationGuid);

        public async Task<List<Application>> GetAllAsync()
            => await _dal.GetAllAsync();

        public async Task<Application> CreateAsync(Application application)
        {
            application.CreateSetupConfiguration(_currentUser);
            await _dal.AddAsync(application);
            await _db.SaveChangesAsync();
            return application;
        }

        public async Task UpdateAsync(Application application)
        {
            application.CreateSetupConfiguration(_currentUser);
            await _dal.UpdateAsync(application);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid applicationGuid)
        {

            var entity = await _dal.GetByGuidAsync(applicationGuid);
            if (entity == null) return;

            await _dal.SoftDeleteAsync(entity);
            await _db.SaveChangesAsync();
        }

        #region Paging

        public async Task<PagedResult<Application>> GetPagedAsync(
        int pageNumber,
        int pageSize)
        {
            return await _dal.GetPagedAsync(pageNumber, pageSize);
        }

        public async Task<PagedResult<Application>> GetPagedWithModulesAsync(
        int pageNumber,
        int pageSize)
        {
            return await _dal.GetPagedWithModulesAsync(pageNumber, pageSize);
        }

        public async Task<List<Application>> GetScrollAsync(
        int lastId,
        int batchSize)
        {
            return await _dal.GetScrollAsync(lastId, batchSize);
        }

        #endregion
    }
}
