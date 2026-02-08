using Microsoft.EntityFrameworkCore;
using SchemaAI.DALContracts;
using SchemaAI.Entities;
using SchemaAI.Persistence;

namespace SchemaAI.DAL
{
    public sealed class GenAIProviderDal
    : BaseDal<GenAIProvider>, IGenAIProviderDal
    {
        public GenAIProviderDal(SchemaAIDbContext db) : base(db) { }       
    }
}
