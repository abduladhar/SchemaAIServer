using CommonEntities;
using SchemaAI.Entities;

namespace SchemaAI.ServiceContract
{
    public interface IUserService
    {
        Task<List<User>> GetAllAsync();
        Task<User?> GetAsync(Guid UserGuid);
        Task<User> CreateAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(Guid UserGuid);
        Task<User> LoginAsync(LoginModel p_objLoginModel);
        Task ResetPassword(LoginModel p_objLoginModel);
        Task ChangePassword(User user);

        #region Paging

        Task<PagedResult<User>> GetPagedAsync(int pageNumber, int pageSize);

        Task<List<User>> GetScrollAsync(int lastId, int batchSize);

        #endregion Paging
    }
}
