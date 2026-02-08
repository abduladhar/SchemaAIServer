using Microsoft.EntityFrameworkCore;
using SchemaAI.DALContracts;
using SchemaAI.Entities;
using SchemaAI.Persistence;

namespace SchemaAI.DAL
{
    public sealed class UserDal
    : BaseDal<User>, IUserDal
    {
        public UserDal(SchemaAIDbContext db) : base(db) { }

        public async Task<User?> GetUserByUsername(string userName, CancellationToken cancellationToken = default)
        {
            return await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    u => u.UserName == userName,
                    cancellationToken
                );
        }

        public async Task<User?> GetUserByEmail(string email, CancellationToken cancellationToken = default)
        {
            return await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    u => u.Email == email,
                    cancellationToken
                );
        }

    }
}
