using SchemaAI.Entities;

namespace SchemaAI.ServiceContract
{
    public interface ILanguageModelService
    {
        Task<List<LanguageModel>> GetAllAsync();
        Task<LanguageModel?> GetAsync(Guid LanguageModelGuid);
        Task<LanguageModel> CreateAsync(LanguageModel LanguageModel);
        Task UpdateAsync(LanguageModel LanguageModel);
        Task DeleteAsync(Guid LanguageModelGuid);
        Task<List<LanguageModel>> GetAllLanguageModelAsync(string szProviderName);
        Task<List<LanguageModel>> GetAllLanguageModelAsync(string szProviderName, bool isActive);
    }
}
