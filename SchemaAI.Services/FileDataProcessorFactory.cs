using Microsoft.Extensions.DependencyInjection;
using SchemaAI.Entities;
using SchemaAI.ServiceContract;
using System.Threading.Tasks;

namespace SchemaAI.Services
{
    public sealed class FileDataProcessorFactory : IFileDataProcessorFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public FileDataProcessorFactory(IServiceProvider serviceProvider, ISystemConfigService systemConfigService)
        {
            _serviceProvider = serviceProvider;
        }

        public IFileDataProcessor Create(SystemConfig systemConfig)
        {
            if (systemConfig == null)
                throw new ArgumentNullException(nameof(systemConfig));

            return systemConfig.UseAws
                ? _serviceProvider.GetRequiredService<AWSDataProcessor>()
                : _serviceProvider.GetRequiredService<AIProviderDataProcessor>();
        }
    }


}
