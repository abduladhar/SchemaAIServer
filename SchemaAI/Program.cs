using Amazon.S3;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using SchemaAI;
using SchemaAI.DAL;
using SchemaAI.DALContracts;
using SchemaAI.Entities;
using SchemaAI.Middleware;
using SchemaAI.Persistence;
using SchemaAI.ServiceContract;
using SchemaAI.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =====================
// Services
// =====================

builder.Services.AddControllers();

builder.Services.AddDbContext<SchemaAIDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    );
});

builder.Services.AddMemoryCache();

builder.Services.AddAWSService<IAmazonS3>();

// DAL
builder.Services.AddScoped<IApplicationDal, ApplicationDal>();
builder.Services.AddScoped<IModuleDal, ModuleDal>();
builder.Services.AddScoped<ITemplateDal, TemplateDal>();
builder.Services.AddScoped<IProcessingRequestDal, ProcessingRequestDal>();
builder.Services.AddScoped<IUploadedFileDal, UploadedFileDal>();
builder.Services.AddScoped<ITemplatePromptDal, TemplatePromptDal>();
builder.Services.AddScoped<ITemplateFileConfigDal, TemplateFileConfigDal>();
builder.Services.AddScoped<IProcessedResultDal, ProcessedResultDal>();
builder.Services.AddScoped<IAwsConfigDal, AwsConfigDal>();
builder.Services.AddScoped<IGenAIProviderDal, GenAIProviderDal>();
builder.Services.AddScoped<ISystemConfigDal, SystemConfigDal>();
builder.Services.AddScoped<IUserDal, UserDal>();
builder.Services.AddScoped<IEmailSettingDal, EmailSettingDal>();
builder.Services.AddScoped<ILanguageModelDal, LanguageModelDal>();


// Services
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<IModuleService, ModuleService>();
builder.Services.AddScoped<ITemplateService, TemplateService>();
builder.Services.AddScoped<ITemplatePromptService, TemplatePromptService>();
builder.Services.AddScoped<ITemplateFileConfigService, TemplateFileConfigService>();
builder.Services.AddScoped<IUploadedFileService, UploadedFileService>();
builder.Services.AddScoped<IProcessingRequestService, ProcessingRequestService>();
builder.Services.AddScoped<IProcessedResultService, ProcessedResultService>();
builder.Services.AddScoped<IAwsConfigService, AwsConfigService>();
builder.Services.AddScoped<IGenAIProviderService, GenAIProviderService>();
builder.Services.AddScoped<ISystemConfigService, SystemConfigService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailSettingService, EmailSettingService>();
builder.Services.AddScoped<ILanguageModelService, LanguageModelService>();


builder.Services.AddScoped<ICurrentUser, CurrentUser>();

builder.Services.AddScoped<AWSDataProcessor>();
builder.Services.AddScoped<AIProviderDataProcessor>();
builder.Services.AddScoped<IFileDataProcessorFactory, FileDataProcessorFactory>();
builder.Services.AddScoped<IAWSService, AWSService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient();

// =====================
// CORS
// =====================

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// =====================
// JWT Authentication
// =====================

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };
    });

// =====================
// Swagger
// =====================
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SchemaAI API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter: Bearer {JWT token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            },
            new List<string>()
        }
    });
});

// =====================
// App Pipeline
// =====================

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();       // 🔑 MUST come before Authorization

app.UseAuthorization();

app.UseMiddleware<CurrentUserMiddleware>(); // ✅ AFTER auth

app.MapControllers();

app.Run();
