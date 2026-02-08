using SchemaAI.Entities;

namespace SchemaAI.DALContracts
{
    public interface IUploadedFileDal : IBaseDal<UploadedFile>
    {
        Task<UploadedFile?> GetByReferenceAsync(string referenceNumber);
        Task<UploadedFile?> GetUploadedFilebyGuidAsync(Guid p_UploadedFileGuid);
    }
}
