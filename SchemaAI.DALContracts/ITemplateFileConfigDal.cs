using SchemaAI.Entities;

namespace SchemaAI.DALContracts
{
    public interface ITemplateFileConfigDal : IBaseDal<TemplateFileConfig>
    {
        Task<TemplateFileConfig?> GetByTemplateGuidAsync(Guid templateGuid);
    }
}
