using CommonEntities;
using SchemaAI.Entities;

namespace SchemaAI.DALContracts
{
    public interface IApplicationDal : IBaseDal<Application>
    {
        Task<Application?> GetWithModulesAsync(Guid applicationGuid);
        Task<PagedResult<Application>> GetPagedWithModulesAsync(
        int pageNumber,
        int pageSize);
    }
}
