using CommonEntities;
using Microsoft.EntityFrameworkCore;
using SchemaAI.DALContracts;
using SchemaAI.Entities;
using SchemaAI.Persistence;
using SchemaAI.ServiceContract;

namespace SchemaAI.Services
{
    public sealed class TemplateService : ITemplateService
    {
        private readonly ITemplateDal _dal;
        private readonly SchemaAIDbContext _db;
        private readonly ICurrentUser _currentUser;

        public TemplateService(ITemplateDal dal, SchemaAIDbContext db, ICurrentUser currentUser)
        {
            _dal = dal;
            _db = db;
            _currentUser = currentUser;
        }

        public async Task<Template?> GetFullAsync(Guid templateGuid)
            => await _dal.GetFullTemplateAsync(templateGuid);

        public async Task<List<Template>> GetAllAsync()
                       => await _dal.GetAllAsync();

        public async Task<List<Template>> GetByModuleAsync(Guid moduleGuid)
            => await _dal.Query()
                         .Where(x => x.ModuleGuid == moduleGuid)
                         .ToListAsync();

        public async Task<Template> CreateAsync(Template template)
        {
            template.CreateSetupConfiguration(_currentUser);
            await _dal.AddAsync(template);
            await _db.SaveChangesAsync();
            return template;
        }

        public async Task UpdateAsync(Template template)
        {
            template.CreateSetupConfiguration(_currentUser);
            await _dal.UpdateAsync(template);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid templateGuid)
        {
            var entity = await _dal.GetByGuidAsync(templateGuid);
            if (entity == null) return;

            await _dal.SoftDeleteAsync(entity);
            await _db.SaveChangesAsync();
        }

        #region Paging

        public async Task<PagedResult<Template>> GetPagedAsync(
        int pageNumber,
        int pageSize)
        {
            return await _dal.GetPagedAsync(pageNumber, pageSize);
        }

        public async Task<List<Template>> GetScrollAsync(
        int lastId,
        int batchSize)
        {
            return await _dal.GetScrollAsync(lastId, batchSize);
        }

        #endregion
    }
}
