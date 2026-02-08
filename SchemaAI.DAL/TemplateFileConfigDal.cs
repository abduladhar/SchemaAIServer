using Microsoft.EntityFrameworkCore;
using SchemaAI.DALContracts;
using SchemaAI.Entities;
using SchemaAI.Persistence;

namespace SchemaAI.DAL
{
    public sealed class TemplateFileConfigDal
        : BaseDal<TemplateFileConfig>, ITemplateFileConfigDal
    {
        public TemplateFileConfigDal(SchemaAIDbContext db)
            : base(db)
        {
        }

        public async Task<TemplateFileConfig?> GetByTemplateGuidAsync(Guid templateGuid)
        {
            return await _db.TemplateFileConfigs
                .FirstOrDefaultAsync(x => x.TemplateGuid == templateGuid);
        }
    }
}
