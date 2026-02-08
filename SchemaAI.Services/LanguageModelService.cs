using Microsoft.EntityFrameworkCore;
using SchemaAI.DAL;
using SchemaAI.DALContracts;
using SchemaAI.Entities;
using SchemaAI.Persistence;
using SchemaAI.ServiceContract;

namespace SchemaAI.Services
{
    public sealed class LanguageModelService : ILanguageModelService
    {
        private readonly ILanguageModelDal _dal;
        private readonly SchemaAIDbContext _db;
        private readonly ICurrentUser _currentUser;
        public LanguageModelService(ILanguageModelDal dal, SchemaAIDbContext db, ICurrentUser currentUser)
        {
            _dal = dal;
            _db = db;
            _currentUser = currentUser;
        }

        public async Task<List<LanguageModel>> GetAllAsync()
                        => await _dal.GetAllAsync();

       
        public async Task<LanguageModel?> GetAsync(Guid LanguageModelGuid)
            => await _dal.GetByGuidAsync(LanguageModelGuid);

        public async Task<LanguageModel> CreateAsync(LanguageModel LanguageModel)
        {
            LanguageModel.CreateSetupConfiguration(_currentUser);
            await _dal.AddAsync(LanguageModel);
            await _db.SaveChangesAsync();
            return LanguageModel;
        }

        public async Task UpdateAsync(LanguageModel LanguageModel)
        {
            LanguageModel.CreateSetupConfiguration(_currentUser);
            await _dal.UpdateAsync(LanguageModel);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid LanguageModelGuid)
        {
            var entity = await _dal.GetByGuidAsync(LanguageModelGuid);
            if (entity == null) return;

            await _dal.SoftDeleteAsync(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<List<LanguageModel>> GetAllLanguageModelAsync(string szProviderName)
          => await _dal.GetAllLanguageModelAsync(szProviderName);

        public async Task<List<LanguageModel>> GetAllLanguageModelAsync(string szProviderName,bool isActive)
         => await _dal.GetAllLanguageModelAsync(szProviderName);
    }
}
