using SchemaAI.Entities;

namespace SchemaAI.DALContracts
{
    public interface ITemplatePromptDal : IBaseDal<TemplatePrompt>
    {
        Task<TemplatePrompt?> GetByTemplateGuidAsync(Guid templateGuid);
    }
}
