using SchemaAI.Entities;

namespace SchemaAI.DALContracts
{
    public interface IUserDal : IBaseDal<User>
    {
        Task<User?> GetUserByUsername(string userName, CancellationToken cancellationToken = default);
        Task<User?> GetUserByEmail(string email, CancellationToken cancellationToken = default);
    }
}
