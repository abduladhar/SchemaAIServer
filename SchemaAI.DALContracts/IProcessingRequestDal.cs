using SchemaAI.Entities;

namespace SchemaAI.DALContracts
{
    public interface IProcessingRequestDal : IBaseDal<ProcessingRequest>
    {
        Task<ProcessingRequest?> GetByReferenceAsync(string referenceNumber);
    }
}
