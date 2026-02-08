using SchemaAI.Entities;

namespace SchemaAI.DALContracts
{
    public interface ITemplateDal : IBaseDal<Template>
    {
        Task<Template?> GetFullTemplateAsync(Guid templateGuid);
    }
}
