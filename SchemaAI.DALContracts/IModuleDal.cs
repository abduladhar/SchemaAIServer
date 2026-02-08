using SchemaAI.Entities;

namespace SchemaAI.DALContracts
{
    public interface IModuleDal : IBaseDal<Module>
    {
        Task<List<Module>> GetByApplicationGuidAsync(Guid applicationGuid);
    }
}
