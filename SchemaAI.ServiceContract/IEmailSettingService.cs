using CommonEntities;
using SchemaAI.Entities;

namespace SchemaAI.ServiceContract
{
    public interface IEmailSettingService
    {
        Task<List<EmailSetting>> GetAllAsync();
        Task<EmailSetting?> GetAsync(Guid EmailSettingGuid);
        Task<EmailSetting> CreateAsync(EmailSetting EmailSetting);
        Task UpdateAsync(EmailSetting EmailSetting);
        Task DeleteAsync(Guid EmailSettingGuid);
        Task<EmailSetting> GetEmailSettingAsync();

        #region Paging

        Task<PagedResult<EmailSetting>> GetPagedAsync(int pageNumber, int pageSize);

        Task<List<EmailSetting>> GetScrollAsync(int lastId, int batchSize);

        #endregion Paging
    }
}
