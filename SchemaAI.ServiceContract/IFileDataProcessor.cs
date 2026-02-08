using SchemaAI.Entities;

namespace SchemaAI.ServiceContract
{
    public interface IFileDataProcessor
    {
        Task<bool> ProcessAsync(Guid uploadedFileGuid);
    }

    public interface IFileDataProcessorFactory
    {
        IFileDataProcessor Create(SystemConfig p_objSystemConfig);
    }
}
