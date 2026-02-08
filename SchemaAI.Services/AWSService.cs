using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using SchemaAI.ServiceContract;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchemaAI.Services
{
    using Amazon.BedrockRuntime;
    using Amazon.BedrockRuntime.Model;
    using Amazon.S3;
    using Amazon.Textract;
    using Amazon.Textract.Model;
    using SchemaAI.Entities;
    using System.Collections.Concurrent;
    using System.Text.Json;
    using static System.Net.Mime.MediaTypeNames;

    public sealed class AWSService : IAWSService
    {
        private readonly IAwsConfigService _awsConfigService;
        private readonly ConcurrentDictionary<Guid, IAmazonS3> _clients = new();
        private Guid tenantGuid = Guid.Empty;
        private AmazonTextractClient _textract;
        private AmazonBedrockRuntimeClient _amazonBedrockRuntimeClient;
        private readonly ICurrentUser _currentUser;


        public AWSService(IAwsConfigService awsConfigService, ICurrentUser currentUser)
        {
            _awsConfigService = awsConfigService;
            _currentUser = currentUser;
            tenantGuid = currentUser.TenantGuid;
        }

        private async Task<(IAmazonS3 s3, string bucket)> GetClientAsync()
        {
            if (_clients.TryGetValue(tenantGuid, out var client))
            {
                var cfg = await _awsConfigService.GetCurrentAsync();
                return (client, cfg.S3BucketName);
            }

            var config = await _awsConfigService.GetCurrentAsync();

            var credentials = new BasicAWSCredentials(
                config.AccessKeyID,
                config.SecretAccessKey
            );

            // ✅ FIX: Pass credentials to Textract
            _textract = new AmazonTextractClient(
                credentials,
                RegionEndpoint.GetBySystemName(config.Region)
            );

            _amazonBedrockRuntimeClient = new AmazonBedrockRuntimeClient(
                                                    credentials,
                                                    RegionEndpoint.GetBySystemName(config.Region)
                                                );

            var s3Config = new AmazonS3Config
            {
                RegionEndpoint =
                    RegionEndpoint.GetBySystemName(config.Region)
            };

            var s3Client = new AmazonS3Client(credentials, s3Config);

            _clients[tenantGuid] = s3Client;

            return (s3Client, config.S3BucketName);
        }


        //private async Task<(IAmazonS3 s3, string bucket)> GetClientAsync()
        //{
        //    if (_clients.TryGetValue(tenantGuid, out var client))
        //    {
        //        var cfg = await _awsConfigService.GetCurrentAsync();
        //        return (client, cfg.S3BucketName);
        //    }

        //    var config = await _awsConfigService.GetCurrentAsync();

        //    var credentials = new BasicAWSCredentials(
        //        config.AccessKeyID,
        //        config.SecretAccessKey
        //    );

        //    _textract = new AmazonTextractClient(
        //                    RegionEndpoint.GetBySystemName(config.Region)
        //                );

        //    var s3Config = new AmazonS3Config
        //    {
        //        RegionEndpoint =
        //            RegionEndpoint.GetBySystemName(config.Region)
        //    };

        //    var s3Client = new AmazonS3Client(credentials, s3Config);

        //    _clients[tenantGuid] = s3Client;

        //    return (s3Client, config.S3BucketName);
        //}

        public async Task<string> GeneratePutPreSignedUrlAsync(
           string s3Key,
           string contentType,
           int expiryMinutes = 30)
        {
            var (s3, bucket) = await GetClientAsync();

            return s3.GetPreSignedURL(new GetPreSignedUrlRequest
            {
                BucketName = bucket,
                Key = s3Key,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddMinutes(expiryMinutes)
            });
        }

        public async Task<string> GeneratePutPreSignedUrl(
        string s3Key,
        int expiryMinutes = 30)
        {
            var (s3, bucket) = await GetClientAsync();

            var request = new GetPreSignedUrlRequest
            {
                BucketName = bucket,
                Key = s3Key,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddMinutes(expiryMinutes)

            };

            return s3.GetPreSignedURL(request);
        }

        public async Task<string> GenerateGetPreSignedUrl(
            string s3Key,
            int expiryMinutes = 60)
        {
            var (s3, bucket) = await GetClientAsync();

            var request = new GetPreSignedUrlRequest
            {
                BucketName = bucket,
                Key = s3Key,
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.AddMinutes(expiryMinutes)
            };

            return s3.GetPreSignedURL(request);
        }

        public async Task<string> GeneratePutPreSignedUrl(string s3Key, string contentType, int expiryMinutes = 30)
        {
            var (s3, bucket) = await GetClientAsync();

            var request = new GetPreSignedUrlRequest
            {
                BucketName = bucket,
                Key = s3Key,
                Verb = HttpVerb.PUT,
                ContentType = contentType,
                Expires = DateTime.UtcNow.AddMinutes(expiryMinutes)

            };

            return s3.GetPreSignedURL(request);
        }

      

        //public async Task<string> ExtractTextAsync(string key)
        //{
        //    var (s3, bucket) = await GetClientAsync();
        //    var request = new DetectDocumentTextRequest
        //    {
        //        Document = new Document
        //        {
        //            S3Object = new Amazon.Textract.Model.S3Object
        //            {
        //                Bucket = bucket,
        //                Name = key
        //            }
        //        }
        //    };

        //    var response = await _textract.DetectDocumentTextAsync(request);

        //    var sb = new StringBuilder();

        //    foreach (var block in response.Blocks)
        //    {
        //        if (block.BlockType == BlockType.LINE)
        //        {
        //            sb.AppendLine(block.Text);
        //        }
        //    }

        //    return sb.ToString();
        //}

        public async Task<string> ExtractTextAsync(string key, string p_szContentType)
        {
            var (s3, bucket) = await GetClientAsync();

            // 🔍 Pre-validation (VERY IMPORTANT)
            var meta = await s3.GetObjectMetadataAsync(bucket, key);

            if (meta.ContentLength < 1024)
                throw new Exception("Invalid or empty file");

            if (!meta.Headers.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase) &&
                !meta.Headers.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                throw new Exception($"Unsupported content type: {meta.Headers.ContentType}");

            var request = new AnalyzeDocumentRequest
            {
                Document = new Document
                {
                    S3Object = new Amazon.Textract.Model.S3Object
                    {
                        Bucket = bucket,
                        Name = key // ⚠️ no leading slash
                    }
                },
                FeatureTypes = new List<string>
                {
                    "FORMS",
                    "TABLES"
                }
            };

            var response = await _textract.AnalyzeDocumentAsync(request);

            var sb = new StringBuilder();

            foreach (var block in response.Blocks
                                         .Where(b => b.BlockType == BlockType.LINE &&
                                                     !string.IsNullOrWhiteSpace(b.Text)))
            {
                sb.AppendLine(block.Text.Trim());
            }

            return sb.ToString();
        }


        public async Task<string> ExtractTextAsync(string key)
        {
            var (s3, bucket) = await GetClientAsync();

            // 🔍 Pre-validation (VERY IMPORTANT)
            var meta = await s3.GetObjectMetadataAsync(bucket, key);

            if (meta.ContentLength < 1024)
                throw new Exception("Invalid or empty file");

            if (!meta.Headers.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase) &&
                !meta.Headers.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                throw new Exception($"Unsupported content type: {meta.Headers.ContentType}");

            var request = new AnalyzeDocumentRequest
            {
                Document = new Document
                {
                    S3Object = new Amazon.Textract.Model.S3Object
                    {
                        Bucket = bucket,
                        Name = key // ⚠️ no leading slash
                    }
                },
                FeatureTypes = new List<string>
        {
            "FORMS",
            "TABLES"
        }
            };

            var response = await _textract.AnalyzeDocumentAsync(request);

            var sb = new StringBuilder();

            foreach (var block in response.Blocks
                                         .Where(b => b.BlockType == BlockType.LINE &&
                                                     !string.IsNullOrWhiteSpace(b.Text)))
            {
                sb.AppendLine(block.Text.Trim());
            }

            return sb.ToString();
        }


        public async Task<DocumentExtractionResult> ExtractDataAsync(
                        string prompt,
                        string extractedText,
                        CancellationToken cancellationToken = default)
        {



            var requestBody = new
            {
                anthropic_version = "bedrock-2023-05-31",
                max_tokens = 2048,
                temperature = 0,
                messages = new[]
                {
                    new
                    {
                        role = "user",
                        content = new[]
                        {
                            new
                            {
                                type = "text",
                                text = $"""
                                        {prompt}

                                        --- DOCUMENT TEXT START ---
                                        {extractedText}
                                        --- DOCUMENT TEXT END ---
                                        """
                            }
                        }
                    }
                }
            };

            var request = new InvokeModelRequest
            {
                ModelId = "anthropic.claude-3-sonnet-20240229-v1:0",
                ContentType = "application/json",
                Accept = "application/json",
                Body = new MemoryStream(
                    Encoding.UTF8.GetBytes(JsonSerializer.Serialize(requestBody)))
            };

            var response = await _amazonBedrockRuntimeClient.InvokeModelAsync(
                request,
                cancellationToken);

            using var reader = new StreamReader(response.Body);
            var rawJson = await reader.ReadToEndAsync(cancellationToken);

            using var doc = JsonDocument.Parse(rawJson);

            // Claude output path
            var extractedJson =
                doc.RootElement
                   .GetProperty("content")[0]
                   .GetProperty("text")
                   .GetString();

            return new DocumentExtractionResult
            {
                Json = extractedJson ?? "{}",
                RawResponse = rawJson
            };
        }

        public async Task<DocumentExtractionResult> ExtractDataAsync(
                      string prompt,
                      string extractedText, string szModelid,
                      CancellationToken cancellationToken = default)
        {
            string sztext = $"""
                                        {prompt}

                                        --- DOCUMENT TEXT START ---
                                        {extractedText}
                                        --- DOCUMENT TEXT END ---
                                        """;
            int inputToken = EstimateTokens(sztext);

            var requestBody = new
            {
                anthropic_version = "bedrock-2023-05-31",
                max_tokens = inputToken,
                temperature = 0,
                messages = new[]
                {
                    new
                    {
                        role = "user",
                        content = new[]
                        {
                            new
                            {
                                type = "text",
                                text = sztext
                            }
                        }
                    }
                }
            };

            var request = new InvokeModelRequest
            {
                ModelId = szModelid,
                ContentType = "application/json",
                Accept = "application/json",
                Body = new MemoryStream(
                    Encoding.UTF8.GetBytes(JsonSerializer.Serialize(requestBody)))
            };

            var response = await _amazonBedrockRuntimeClient.InvokeModelAsync(
                request,
                cancellationToken);

            using var reader = new StreamReader(response.Body);
            var rawJson = await reader.ReadToEndAsync(cancellationToken);

            using var doc = JsonDocument.Parse(rawJson);

            // Claude output path
            var extractedJson =
                doc.RootElement
                   .GetProperty("content")[0]
                   .GetProperty("text")
                   .GetString();

            return new DocumentExtractionResult
            {
                Json = extractedJson ?? "{}",
                RawResponse = rawJson
            };
        }

        private static int EstimateTokens(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            return (int)Math.Ceiling(text.Length / 3.0);
        }

        public async Task<string> ExtractTextAsync(string key, UploadedFile uploadedFile)
        {
            var (s3, bucket) = await GetClientAsync();


            // 🔍 Always use GetObjectAsync for reliable metadata
            using var obj = await s3.GetObjectAsync(bucket, key);

            var contentType = obj.Headers.ContentType;
            var size = obj.ContentLength;

            if (size < 1024)
                throw new Exception("Invalid or empty file");

            if (!string.Equals(contentType, "application/pdf", StringComparison.OrdinalIgnoreCase) &&
                !contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                throw new Exception($"Unsupported content type: {contentType}");

            // 🔥 Start ASYNC Textract (required for multi-page PDFs)
            var startResponse = await _textract.StartDocumentAnalysisAsync(
                new StartDocumentAnalysisRequest
                {
                    DocumentLocation = new DocumentLocation
                    {
                        S3Object = new Amazon.Textract.Model.S3Object
                        {
                            Bucket = bucket,
                            Name = key // ⚠️ no leading slash
                        }
                    },
                    FeatureTypes = new List<string> { "FORMS", "TABLES" }
                });

            var jobId = startResponse.JobId;

            // ⏳ Wait for job completion
            GetDocumentAnalysisResponse statusResponse;
            do
            {
                //await Task.Delay(2000);
                statusResponse = await _textract.GetDocumentAnalysisAsync(
                    new GetDocumentAnalysisRequest { JobId = jobId });

            } while (statusResponse.JobStatus == JobStatus.IN_PROGRESS);

            if (statusResponse.JobStatus != JobStatus.SUCCEEDED)
                throw new Exception($"Textract failed: {statusResponse.JobStatus}");

            // 📄 Read ALL pages with pagination
            var sb = new StringBuilder();
            string? nextToken = null;

            do
            {
                var response = await _textract.GetDocumentAnalysisAsync(
                    new GetDocumentAnalysisRequest
                    {
                        JobId = jobId,
                        NextToken = nextToken
                    });

                // Group text by page
                var pages = response.Blocks
                    .Where(b => b.BlockType == BlockType.LINE &&
                                !string.IsNullOrWhiteSpace(b.Text))
                    .GroupBy(b => b.Page)
                    .OrderBy(g => g.Key);

                foreach (var page in pages)
                {
                    var lines = page
                        .Select(b => b.Text.Trim())
                        .ToList();

                    // ❌ Skip entire page if it starts with "Terms and Conditions"
                    if (IsTermsAndConditionsPage(lines))
                        continue;

                    foreach (var line in lines)
                        sb.AppendLine(line);
                }

                nextToken = response.NextToken;

            } while (!string.IsNullOrEmpty(nextToken));

            return sb.ToString();
        }


        private static bool IsTermsAndConditionsPage(List<string> lines)
        {
            var firstLine = lines
                .FirstOrDefault(l => !string.IsNullOrWhiteSpace(l))
                ?.Trim()
                .ToLowerInvariant();

            if (string.IsNullOrEmpty(firstLine))
                return false;

            return firstLine.StartsWith("terms and condition");
        }



    }
}
