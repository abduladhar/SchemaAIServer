using CommonEntities;
using SchemaAI.DALContracts;
using SchemaAI.Entities;
using SchemaAI.Persistence;
using SchemaAI.ServiceContract;

namespace SchemaAI.Services
{
    public sealed class UserService : IUserService
    {
        private readonly IUserDal _dal;
        private readonly SchemaAIDbContext _db;
        private readonly IEmailSettingService _emailSettingService;
        private readonly ICurrentUser _currentUser;

        public UserService(IUserDal dal,IEmailSettingService emailSettingService, SchemaAIDbContext db, ICurrentUser currentUser)
        {
            _dal = dal;
            _db = db;
            _emailSettingService = emailSettingService;
            _currentUser = currentUser;
        }

        public async Task<List<User>> GetAllAsync()
                        => await _dal.GetAllAsync();
        public async Task<User?> GetAsync(Guid UserGuid)
            => await _dal.GetByGuidAsync(UserGuid);

        public async Task<User> CreateAsync(User user)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.PasswordHash = hashedPassword;
            user.CreatedOn = DateTime.Now.ToUniversalTime();
            user.CreateSetupConfiguration(_currentUser);
            await _dal.AddAsync(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task UpdateAsync(User user)
        {
            user.CreateSetupConfiguration(_currentUser);
            if (user.Password != string.Empty)
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
                user.PasswordHash = hashedPassword;
            }
            else
            {
                var tempUser = await this.GetAsync(user.UserGuid);
                if (tempUser != null)
                {
                    user.PasswordHash = tempUser.PasswordHash;
                }
            }
;

            await _dal.UpdateAsync(user);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid UserGuid)
        {
            var entity = await _dal.GetByGuidAsync(UserGuid);
            if (entity == null) return;

            await _dal.SoftDeleteAsync(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<User> LoginAsync(LoginModel p_objLoginModel)
        {
            // Fetch user by username or email
            var user = await this.GetByUserNameAsync(p_objLoginModel.Username)
                       ?? await this.GetByEmailAsync(p_objLoginModel.Username);

            if (user == null)
                throw new Exception("Invalid username or password");

            // Compare entered password with stored hash
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(p_objLoginModel.Password, user.PasswordHash);

            if (!isPasswordValid)
                throw new Exception("Invalid username or password");

            user.PasswordHash = string.Empty;
            // Get role details
       

            return user;
        }

        public async Task ResetPassword(LoginModel loginModel)
        {
            var user = await this.GetByEmailAsync(loginModel.Username);
            if (user == null)
                throw new Exception("User not found");

            string newPassword = PasswordHelper.GenerateRandomPassword(8);

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.PasswordHash = hashedPassword;

            await this.UpdateAsync(user);

            string emailSubject = "Your password has been reset";
            string emailBody = $@"
                                Hello {user.UserName},
                                Your password has been reset. Your new password is:  {newPassword}
                                Please log in and change it immediately.";

            EmailSetting objEmailSettings = await _emailSettingService.GetEmailSettingAsync();

            var emailService = new EmailService(objEmailSettings); // Make sure _emailSettings is injected
            await emailService.SendEmailAsync(user.Email, emailSubject, emailBody);
        }

        public async Task ChangePassword(User user)
        {
            User objUser = await this.GetAsync(user.UserGuid) ?? new User();

            if (objUser != null && objUser.UserGuid != Guid.Empty)
            {
                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(user.Password, objUser.PasswordHash);
                if (isPasswordValid)
                {
                    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.NewPassword);
                    objUser.PasswordHash = hashedPassword;
                    await this.UpdateAsync(objUser);

                    string emailSubject = "Your password has been reset";
                    string emailBody = $@"
                                Hello {user.UserName},
                                Your password has been reset successfully. If you did not request this change, please contact our support team immediately.";

                    EmailSetting objEmailSettings = await _emailSettingService.GetEmailSettingAsync();

                    var emailService = new EmailService(objEmailSettings); // Make sure _emailSettings is injected
                    await emailService.SendEmailAsync(objUser.Email, emailSubject, emailBody);
                }
            }


        }

        private async Task<User?> GetByEmailAsync(string email)
        {
            return await _dal.GetUserByEmail(email);
        }

        private async Task<User?> GetByUserNameAsync(string username)
        {
            return await _dal.GetUserByUsername(username);
        }

        #region Paging

        public async Task<PagedResult<User>> GetPagedAsync(
        int pageNumber,
        int pageSize)
        {
            return await _dal.GetPagedAsync(pageNumber, pageSize);
        }

        public async Task<List<User>> GetScrollAsync(
        int lastId,
        int batchSize)
        {
            return await _dal.GetScrollAsync(lastId, batchSize);
        }

        #endregion


    }
}
