using SchemaAI.Entities;

namespace SchemaAI.DALContracts
{
    public interface ISystemConfigDal : IBaseDal<SystemConfig>
    {
        Task<SystemConfig?> GetByApiKey(string apiKey);
    }
}
