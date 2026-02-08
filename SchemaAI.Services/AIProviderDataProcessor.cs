using Amazon.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using SchemaAI.DAL;
using SchemaAI.DALContracts;
using SchemaAI.Entities;
using SchemaAI.Persistence;
using SchemaAI.ServiceContract;
using System;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;


namespace SchemaAI.Services
{
    public sealed class AIProviderDataProcessor : IFileDataProcessor
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

        public AIProviderDataProcessor(HttpClient httpClient,
            IServiceScopeFactory scopeFactory,
            IHttpClientFactory httpClientFactory,
            ILogger<AIProviderDataProcessor> logger,
            IGenAIProviderService genAIProviderService, 
            IUploadedFileService uploadFileService, 
            IAWSService AWSService,
            IProcessedResultService processedResultService,
            ISystemConfigService systemConfigService,
            IProcessingRequestService processingResultService,
            ITemplateService templateService, ILanguageModelService languageModelService,ICurrentUser currentUser)
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
            var objOpenAIprovider = await _GenAIProviderService.GetAllAsync();
            if(objOpenAIprovider.Count>0)
            {
                var uploadedFile = await _uploadFileService.GetUploadedFileByGuid(uploadedFileGuid);
                if (uploadedFile != null)
                {
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
                    string signedUrl = await _AWSService.GenerateGetPreSignedUrl(uploadedFile.StoredFilePath, 65);
                    _httpClient.BaseAddress = new Uri(objOpenAIprovider[0].BaseUrl);
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", objOpenAIprovider[0].SecretAccessKey);
                    DocumentExtractionResult aiResponse = new DocumentExtractionResult();
                    string fileId = string.Empty;
                    if (IsPdf(uploadedFile.ContantType))
                    {
                        fileId = await this.UploadPdfToOpenAI(objOpenAIprovider[0], signedUrl, uploadedFile.ContantType);
                        aiResponse = await this.ExtractFromPdfAsync(objOpenAIprovider[0], szPrompt, fileId);
                    }
                    else if (IsImage(uploadedFile.ContantType)){
                        aiResponse = await this.ExtractFromImageAsync(objOpenAIprovider[0], szPrompt, signedUrl);
                    }
                    try
                    {
                        ProcessedResult objProcessedResult = new ProcessedResult();
                        objProcessedResult.ProcessingRequestGuid = objProcessingRequest.ProcessingRequestGuid;
                        objProcessedResult.CreatedOn = DateTime.UtcNow;
                        objProcessedResult.OutputJson = aiResponse.Json;
                        objProcessedResult.TemplateName = uploadedFile.TemplateName;
                        objProcessedResult.ReferenceNumber = uploadedFile.ReferenceNumber;
                        objProcessedResult.CreateSetupConfiguration(_currentUser);
                        await _processedResultService.CreateAsync(objProcessedResult);

                        objProcessingRequest.Status = ProcessingStatusConst.Completed;
                        await _processingRequestService.UpdateStatusAsync(objProcessingRequest.ProcessingRequestGuid, ProcessingStatusConst.Completed);
                        
                        if (IsPdf(uploadedFile.ContantType))
                        {
                            await DeleteFileAsync(objOpenAIprovider[0], fileId);
                        }

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
                    }
    
                    catch(Exception ex)
                    {
                        objProcessingRequest.Status = ProcessingStatusConst.Failed;
                        await _processingRequestService.UpdateStatusAsync(objProcessingRequest.ProcessingRequestGuid, ProcessingStatusConst.Failed);
                        await DeleteFileAsync(objOpenAIprovider[0], fileId);
                        throw new Exception(ex.Message);
                    }
                    //Store promt in db
                }



            }

            return true;
        }

        private static bool IsImage(string contentType)
        {
            return contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsPdf(string contentType)
        {
            return contentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<string> UploadPdfToOpenAI(GenAIProvider objGenAiProvider,
        string presigenedUrl,string contentType,
        CancellationToken cancellationToken = default)
        {
            // Download PDF
            var pdfBytes = await DownloadFromPresignedUrlAsync(presigenedUrl);
            
            using var multipart = new MultipartFormDataContent();

            // File content
            var fileContent = new ByteArrayContent(pdfBytes);
            fileContent.Headers.ContentType =
                new MediaTypeHeaderValue(contentType);

            var fileName = Path.GetFileName(
                new Uri(presigenedUrl).AbsolutePath);

            multipart.Add(
                fileContent,
                "file",
                string.IsNullOrWhiteSpace(fileName)
                    ? "document.pdf"
                    : fileName);

            // Required by OpenAI Files API
            multipart.Add(new StringContent("assistants"), "purpose");
            var baseUri = new Uri(objGenAiProvider.BaseUrl);
            var endpoint = new Uri(baseUri, "files");

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
               endpoint);

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", objGenAiProvider.SecretAccessKey);

            request.Content = multipart;

            using var response = await _httpClient.SendAsync(
                request,
                cancellationToken);

            var responseJson = await response.Content.ReadAsStringAsync(
                cancellationToken);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException(
                    $"OpenAI file upload failed: {responseJson}");

            using var document = JsonDocument.Parse(responseJson);

            return document.RootElement
                           .GetProperty("id")
                           .GetString()!;
        }

        public async Task<byte[]> DownloadFromPresignedUrlAsync(
        string presignedUrl,
        CancellationToken cancellationToken = default)
        {
            HttpClient objHttpClient = new HttpClient();
            using var request = new HttpRequestMessage(
                HttpMethod.Get,
                presignedUrl);

            // 🚨 CRITICAL: remove Authorization header
            request.Headers.Authorization = null;

            using var response = await objHttpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new Exception($"S3 download failed: {error}");
            }

            return await response.Content.ReadAsByteArrayAsync(cancellationToken);
        }



        public async Task<DocumentExtractionResult> ExtractFromPdfAsync(GenAIProvider objGenAiProvider,
        string prompt,
        string fileId,
        CancellationToken cancellationToken = default)
        {
            var objLanguageModels = await _languageModelService.GetAllLanguageModelAsync(ModelProviders.OpenAI, true);
            LanguageModel objCurrentModel = new LanguageModel();
            if (objLanguageModels.Count > 0)
            {
                objCurrentModel = objLanguageModels[0];
            }

            var requestBody = new
            {
                model = objCurrentModel.ModelIdentifier,
                input = new[]
                {
                    new
                    {
                        role = "user",
                        content = new object[]
                        {
                            new
                            {
                                type = "input_text",
                                text = prompt
                            },
                            new
                            {
                                type = "input_file",
                                file_id = fileId
                            }
                        }
                    }
                }
            };

            var baseUri = new Uri(objGenAiProvider.BaseUrl);
            var endpoint = new Uri(baseUri, "responses");

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                endpoint);

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", objGenAiProvider.SecretAccessKey);

            request.Content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.SendAsync(request, cancellationToken);
            var rawJson = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
                throw new Exception(rawJson);

            using var doc = JsonDocument.Parse(rawJson);

            var extractedText =
                doc.RootElement
                   .GetProperty("output")[0]
                   .GetProperty("content")[0]
                   .GetProperty("text")
                   .GetString();

            return new DocumentExtractionResult
            {
                Json = extractedText ?? "{}",
                RawResponse = rawJson
            };
        }

        public async Task<DocumentExtractionResult> ExtractFromImageAsync(
        GenAIProvider objGenAiProvider,
        string prompt,
        string presignedImageUrl,
        CancellationToken cancellationToken = default)
        {
            var models = await _languageModelService
                .GetAllLanguageModelAsync(ModelProviders.OpenAI, true);

            if (models.Count == 0)
                throw new Exception("No active OpenAI model configured.");

            var model = models[0];

            var requestBody = new
            {
                model = model.ModelIdentifier,
                input = new[]
                {
            new
            {
                role = "user",
                content = new object[]
                {
                    new
                    {
                        type = "input_text",
                        text = prompt
                    },
                    new
                    {
                        type = "input_image",
                        image_url = presignedImageUrl
                    }
                }
            }
        }
            };

            var baseUri = new Uri(objGenAiProvider.BaseUrl.TrimEnd('/') + "/");
            var endpoint = new Uri(baseUri, "responses");

            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", objGenAiProvider.SecretAccessKey);

            request.Content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.SendAsync(request, cancellationToken);
            var rawJson = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
                throw new Exception(rawJson);

            using var doc = JsonDocument.Parse(rawJson);

            var extractedText =
                doc.RootElement
                   .GetProperty("output")[0]
                   .GetProperty("content")[0]
                   .GetProperty("text")
                   .GetString();

            return new DocumentExtractionResult
            {
                Json = extractedText ?? "{}",
                RawResponse = rawJson
            };
        }



        public async Task DeleteFileAsync(GenAIProvider objGenAiProvider,
       string fileId,
       CancellationToken cancellationToken = default)
        {
            HttpClient newhttpClient = new HttpClient();
            newhttpClient.BaseAddress = new Uri(objGenAiProvider.BaseUrl);
            newhttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", objGenAiProvider.SecretAccessKey);
            using var response = await _httpClient.DeleteAsync(
                $"files/{fileId}",
                cancellationToken);

            response.EnsureSuccessStatusCode();
        }

    }


    //public class ApiResult
    //{
    //    public string Status { get; set; } = string.Empty;
    //    public string Data { get; set; } = string.Empty;
    //}

    //public class ApiResultEntity
    //{
    //    public int Id { get; set; }
    //    public Guid RequestId { get; set; }
    //    public string ResultData { get; set; } = string.Empty;
    //    public DateTime CreatedAt { get; set; }
    //}
}
