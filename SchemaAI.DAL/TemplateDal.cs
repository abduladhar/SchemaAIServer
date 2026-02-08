using Microsoft.EntityFrameworkCore;
using SchemaAI.DALContracts;
using SchemaAI.Entities;
using SchemaAI.Persistence;

namespace SchemaAI.DAL
{
    public sealed class TemplateDal
    : BaseDal<Template>, ITemplateDal
    {
        public TemplateDal(SchemaAIDbContext db) : base(db) { }

        public async Task<Template?> GetFullTemplateAsync(Guid templateGuid)
        {
            return await _db.Templates
                .Include(t => t.Prompt)
                .Include(t => t.FileConfig)
                .FirstOrDefaultAsync(t => t.TemplateGuid == templateGuid);
        }
    }
}
