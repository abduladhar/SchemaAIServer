using CommonEntities;
using SchemaAI.DALContracts;
using SchemaAI.Entities;
using SchemaAI.Persistence;
using SchemaAI.ServiceContract;

namespace SchemaAI.Services
{
    public sealed class TemplatePromptService : ITemplatePromptService
    {
        private readonly ITemplatePromptDal _dal;
        private readonly SchemaAIDbContext _db;
        private readonly ICurrentUser _currentUser;

        public TemplatePromptService(
            ITemplatePromptDal dal,
            SchemaAIDbContext db,
            ICurrentUser currentUser)
        {
            _dal = dal;
            _db = db;
            _currentUser = currentUser;
        }

        public async Task<TemplatePrompt?> GetByTemplateAsync(Guid templateGuid)
            => await _dal.GetByTemplateGuidAsync(templateGuid);

        public async Task<TemplatePrompt> CreateOrUpdateAsync(TemplatePrompt prompt)
        {
            var existing = await _dal.GetByTemplateGuidAsync(prompt.TemplateGuid);

            if (existing == null)
            {
                prompt.CreateSetupConfiguration(_currentUser);
                await _dal.AddAsync(prompt);
            }
            else
            {
                prompt.CreateSetupConfiguration(_currentUser);
                existing.PromptText = prompt.PromptText;
                existing.OutputSchemaJson = prompt.OutputSchemaJson;
                await _dal.UpdateAsync(existing);
            }

            await _db.SaveChangesAsync();
            return prompt;
        }

        #region Paging

        public async Task<PagedResult<TemplatePrompt>> GetPagedAsync(
        int pageNumber,
        int pageSize)
        {
            return await _dal.GetPagedAsync(pageNumber, pageSize);
        }

        public async Task<List<TemplatePrompt>> GetScrollAsync(
        int lastId,
        int batchSize)
        {
            return await _dal.GetScrollAsync(lastId, batchSize);
        }

        #endregion
    }
}
