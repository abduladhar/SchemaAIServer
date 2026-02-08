using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SchemaAI.DALContracts;
using SchemaAI.Entities;
using SchemaAI.Persistence;
using SchemaAI.ServiceContract;
using System.Net.Http.Json;

namespace SchemaAI.Services
{
    public sealed class AWSDataProcessor : IFileDataProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AIProviderDataProcessor> _logger;
        private readonly HttpClient _httpClient;
        private readonly IGenAIProviderService _GenAIProviderService;
        private readonly IUploadedFileService _uploadFileService;
        private readonly IAWSService _AWSService;
        private readonly IProcessingRequestService _processingRequestService;
        private readonly IProcessedResultService _processedResultService;
        private readonly ITemplateService _templateService;
        private readonly ISystemConfigService _SystemConfigService;
        private readonly ILanguageModelService _languageModelService;
        private readonly ICurrentUser _currentUser;


        public AWSDataProcessor(HttpClient httpClient,
            IServiceScopeFactory scopeFactory,
            IHttpClientFactory httpClientFactory,
            ILogger<AIProviderDataProcessor> logger,
            IGenAIProviderService genAIProviderService,
            IUploadedFileService uploadFileService,
            IAWSService AWSService,
            IProcessedResultService processedResultService,
            IProcessingRequestService processingResultService,
            ITemplateService templateService,
            ISystemConfigService systemConfigService,
            ILanguageModelService languageModelService,
            ICurrentUser currentUser)
        {
            _httpClient = httpClient;
            _scopeFactory = scopeFactory;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _GenAIProviderService = genAIProviderService;
            _uploadFileService = uploadFileService;
            _AWSService = AWSService;
            _processedResultService = processedResultService;
            _processingRequestService = processingResultService;
            _templateService = templateService;
            _SystemConfigService = systemConfigService;
            _languageModelService = languageModelService;
            _currentUser = currentUser;
        }

        public async Task<bool> ProcessAsync(Guid uploadedFileGuid)
        {
            // Read Uploaded File
            var uploadedFile = await _uploadFileService.GetUploadedFileByGuid(uploadedFileGuid);
            if (uploadedFile != null)
            {
                var objLanguageModels = await _languageModelService.GetAllLanguageModelAsync(ModelProviders.AWS, true);
                LanguageModel objCurrentModel = new LanguageModel();
                if (objLanguageModels.Count > 0)
                {
                    objCurrentModel = objLanguageModels[0];
                }

                // Read Template
                ProcessingRequest objProcessingRequest = new ProcessingRequest();
                objProcessingRequest.CreatedOn = DateTime.UtcNow;
                objProcessingRequest.Status = ProcessingStatusConst.Processing;
                objProcessingRequest.ReferenceNumber = uploadedFile.ReferenceNumber;
                objProcessingRequest.TemplateGuid = uploadedFile.TemplateGuid;
                objProcessingRequest.UploadedFileGuid = uploadedFile.UploadedFileGuid;
                objProcessingRequest.ProcessingRequestGuid = Guid.NewGuid();
                objProcessingRequest.TemplateName = uploadedFile.TemplateName;
                objProcessingRequest.CreateSetupConfiguration(_currentUser);


                var objTemplate = await _templateService.GetFullAsync(uploadedFile.TemplateGuid);
                string szPrompt = objTemplate.Prompt.PromptText;
                objProcessingRequest = await _processingRequestService.CreateAsync(objProcessingRequest);
                string extractedText = await _AWSService.ExtractTextAsync(uploadedFile.StoredFilePath, uploadedFile);
                var objAIResponse = await _AWSService.ExtractDataAsync(szPrompt,extractedText, objCurrentModel.ModelIdentifier);

                ProcessedResult objProcessedResult = new ProcessedResult();
                objProcessedResult.ProcessingRequestGuid = objProcessingRequest.ProcessingRequestGuid;
                objProcessedResult.CreatedOn = DateTime.UtcNow;
                objProcessedResult.OutputJson = objAIResponse.Json;
                objProcessedResult.TemplateName = uploadedFile.TemplateName;
                objProcessedResult.ReferenceNumber = uploadedFile.ReferenceNumber;
                objProcessedResult.CreateSetupConfiguration(_currentUser);
                await _processedResultService.CreateAsync(objProcessedResult);

                objProcessingRequest.Status = ProcessingStatusConst.Completed;
                await _processingRequestService.UpdateStatusAsync(objProcessingRequest.ProcessingRequestGuid, ProcessingStatusConst.Completed);
                var SystemConfigs = await _SystemConfigService.GetAllAsync();
               
                if (SystemConfigs != null && SystemConfigs.Count > 0)
                {
                    HttpClient objHttpClient = new HttpClient();
                    string szWebhookurl = SystemConfigs[0].WebhookUrl;
                    var objWebhookResult = new WebhookResult
                    {
                        ReferenceNumber = objProcessedResult.ReferenceNumber,
                        OutputJson = objProcessedResult.OutputJson
                    };

                    var response = await objHttpClient.PostAsJsonAsync(szWebhookurl, objWebhookResult);

                    if (!response.IsSuccessStatusCode)
                    {
                        // optional: log or handle failure
                        var error = await response.Content.ReadAsStringAsync();
                        // logger.LogError("Webhook failed: {Error}", error);
                    }
                }
                // Upload file to OpenAI
                // Read Prompt
                // Request OpenAI
                // Save Result to DB
            }
            return true;
        }
    }
}
