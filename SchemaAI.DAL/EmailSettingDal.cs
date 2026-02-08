using Microsoft.EntityFrameworkCore;
using SchemaAI.DALContracts;
using SchemaAI.Entities;
using SchemaAI.Persistence;

namespace SchemaAI.DAL
{
    public sealed class EmailSettingDal
    : BaseDal<EmailSetting>, IEmailSettingDal
    {
        public EmailSettingDal(SchemaAIDbContext db) : base(db) { }
    }
}
