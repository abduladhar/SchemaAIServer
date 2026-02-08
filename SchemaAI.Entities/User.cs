using SchemaAI.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SchemaAI.Entities
{
    public class User : BaseEntity, IIgnoreTenantFilter
    {
        public Guid UserGuid { get; set; }
        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        [NotMapped]
        public string Password { get; set; } = string.Empty;

        [NotMapped]
        public string NewPassword { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

      
    }

    public class LoginModel
    {
        public string Username { get; set; } = "";

        public string Password { get; set; } = "";

        public string NewPassword { get; set; } = "";
    }
}
