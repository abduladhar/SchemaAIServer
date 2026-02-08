using System;
using System.Collections.Generic;
using System.Text;

namespace SchemaAI.Entities.Common
{
    public interface ITenantEntity
    {
        Guid TenantGuid { get; set; }
    }

}
