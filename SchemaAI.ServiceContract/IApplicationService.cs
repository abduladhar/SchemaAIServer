using CommonEntities;
using SchemaAI.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchemaAI.ServiceContract
{
    public interface IApplicationService
    {
        Task<Application?> GetAsync(Guid applicationGuid);
        Task<List<Application>> GetAllAsync();
        Task<Application> CreateAsync(Application application);
        Task UpdateAsync(Application application);
        Task DeleteAsync(Guid applicationGuid);

        #region Paging

        Task<PagedResult<Application>> GetPagedAsync(int pageNumber, int pageSize);

        Task<PagedResult<Application>> GetPagedWithModulesAsync(
            int pageNumber,
            int pageSize);

        Task<List<Application>> GetScrollAsync(int lastId, int batchSize);

        #endregion Paging
    }
}
