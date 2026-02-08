using CommonEntities;
using SchemaAI.DALContracts;
using SchemaAI.Entities;
using SchemaAI.Persistence;
using SchemaAI.ServiceContract;

namespace SchemaAI.Services
{
    public sealed class EmailSettingService : IEmailSettingService
    {
        private readonly IEmailSettingDal _dal;
        private readonly SchemaAIDbContext _db;
        private readonly ICurrentUser _currentUser;

        public EmailSettingService(IEmailSettingDal dal, SchemaAIDbContext db, ICurrentUser currentUser)
        {
            _dal = dal;
            _db = db;
            _currentUser = currentUser;
        }

        public async Task<List<EmailSetting>> GetAllAsync()
                        => await _dal.GetAllAsync();

        public async Task<EmailSetting> GetEmailSettingAsync()
        {
            List<EmailSetting> objItems = await _dal.GetAllAsync();
            if(objItems.Count>0)
            {
                return objItems[0];
            }
            else
            {
               return new EmailSetting();
            }
        }
                        
        public async Task<EmailSetting?> GetAsync(Guid EmailSettingGuid)
            => await _dal.GetByGuidAsync(EmailSettingGuid);

     
        public async Task<EmailSetting> CreateAsync(EmailSetting EmailSetting)
        {
            EmailSetting.CreateSetupConfiguration(_currentUser);
            await _dal.AddAsync(EmailSetting);
            await _db.SaveChangesAsync();
            return EmailSetting;
        }

        public async Task UpdateAsync(EmailSetting EmailSetting)
        {
            EmailSetting.CreateSetupConfiguration(_currentUser);
            await _dal.UpdateAsync(EmailSetting);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid EmailSettingGuid)
        {
            var entity = await _dal.GetByGuidAsync(EmailSettingGuid);
            if (entity == null) return;

            await _dal.SoftDeleteAsync(entity);
            await _db.SaveChangesAsync();
        }

        #region Paging

        public async Task<PagedResult<EmailSetting>> GetPagedAsync(
        int pageNumber,
        int pageSize)
        {
            return await _dal.GetPagedAsync(pageNumber, pageSize);
        }

        public async Task<List<EmailSetting>> GetScrollAsync(
        int lastId,
        int batchSize)
        {
            return await _dal.GetScrollAsync(lastId, batchSize);
        }

        #endregion
    }
}
