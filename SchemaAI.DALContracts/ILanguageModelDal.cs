using SchemaAI.Entities;

namespace SchemaAI.DALContracts
{
    public interface ILanguageModelDal : IBaseDal<LanguageModel>
    {
        Task<List<LanguageModel>> GetAllLanguageModelAsync(string szProviderName);
        Task<List<LanguageModel>> GetAllLanguageModelAsync(string szProviderName, bool isActive);
    }
}
