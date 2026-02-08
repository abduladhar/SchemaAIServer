using SchemaAI.Entities;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace SchemaAI.ServiceContract
{
    public interface IAWSService
    {
        Task<string> GenerateGetPreSignedUrl(string s3Key,int expiryMinutes = 60);
        Task<string> GeneratePutPreSignedUrl(string s3Key,string contentType,int expiryMinutes = 30);
        Task<string> ExtractTextAsync(string key);
        Task<DocumentExtractionResult> ExtractDataAsync(
                        string prompt,
                        string extractedText,
                        CancellationToken cancellationToken = default);

        Task<DocumentExtractionResult> ExtractDataAsync(
                      string prompt,
                      string extractedText, string szModelid,
                      CancellationToken cancellationToken = default);
        Task<string> ExtractTextAsync(string key,UploadedFile szUploadedFile);

    }
}
