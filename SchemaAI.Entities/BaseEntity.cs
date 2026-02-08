using SchemaAI.Entities.Common;
using System;
using System.Collections.Generic;

namespace SchemaAI.Entities
{
    public abstract class BaseEntity : ITenantEntity, IUserAudit, ISoftDelete
    {
        public int Id { get; set; }
        public Guid TenantGuid { get; set; }

        public Guid CreatedByUserGuid { get; set; }
        public string CreatedByUserName { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }

        public Guid? ModifiedByUserGuid { get; set; }
        public string? ModifiedByUserName { get; set; } = string.Empty;
        public DateTime? ModifiedOn { get; set; }

        public bool IsDeleted { get; set; }

        public void CreateSetupConfiguration(ICurrentUser currentUser)
        {
            CreatedOn = DateTime.UtcNow;
            ModifiedOn = DateTime.UtcNow;
            IsDeleted = false;

            CreatedByUserGuid = currentUser.UserGuid;
            CreatedByUserName = currentUser.UserName;

            ModifiedByUserGuid = currentUser.UserGuid;
            ModifiedByUserName = currentUser.UserName;

            TenantGuid = currentUser.TenantGuid;
        }
    }

    //public interface ICurrentUser
    //{
    //    Guid UserGuid { get; set; }
    //    string UserName { get; set; }
    //    string Phone { get; set; }
    //    Guid TenantGuid { get; set; }
    //    bool IsAuthenticated { get; set; }
    //}


    //public class CurrentUser : ICurrentUser
    //{
    //    public Guid UserGuid { get; set; } = Guid.Empty;
    //    public string UserName { get; set; } = string.Empty;
    //    public Guid TenantGuid { get; set; } = Guid.Empty;
    //    public bool IsAuthenticated { get; set; }
    //    public string Phone { get ; set ; } = string.Empty;
    //}


}
