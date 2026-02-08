using Microsoft.EntityFrameworkCore;
using SchemaAI.DALContracts;
using SchemaAI.Entities;
using SchemaAI.Persistence;

namespace SchemaAI.DAL
{
    public sealed class AwsConfigDal
    : BaseDal<AwsConfig>, IAwsConfigDal
    {
        public AwsConfigDal(SchemaAIDbContext db) : base(db) { }       
    }
}
