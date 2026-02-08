using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SchemaAI.Entities;
using SchemaAI.Entities.Common;
using System.Linq.Expressions;

namespace SchemaAI.Persistence
{
    public sealed class SchemaAIDbContext : DbContext
    {
        // Set this from middleware / service
        private readonly ICurrentUser _currentUser;

        // EF Core reads this dynamically
        public Guid CurrentTenantGuid =>
            _currentUser?.TenantGuid ?? Guid.Empty;

        public SchemaAIDbContext(
            DbContextOptions<SchemaAIDbContext> options,
            ICurrentUser currentUser)
            : base(options)
        {
            _currentUser = currentUser;
        }

        #region DbSets

        public DbSet<Application> Applications => Set<Application>();
        public DbSet<Module> Modules => Set<Module>();
        public DbSet<Template> Templates => Set<Template>();
        public DbSet<TemplatePrompt> TemplatePrompts => Set<TemplatePrompt>();
        public DbSet<TemplateFileConfig> TemplateFileConfigs => Set<TemplateFileConfig>();
        public DbSet<UploadedFile> UploadedFiles => Set<UploadedFile>();
        public DbSet<ProcessingRequest> ProcessingRequests => Set<ProcessingRequest>();
        public DbSet<ProcessedResult> ProcessedResults => Set<ProcessedResult>();
        public DbSet<User> Users => Set<User>();
        public DbSet<EmailSetting> EmailSettings => Set<EmailSetting>();
        public DbSet<LanguageModel> LanguageModels => Set<LanguageModel>();
        public DbSet<SystemConfig> SystemConfig => Set<SystemConfig>();

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ApplyGlobalFilters(modelBuilder);
            ConfigureBaseDefaults(modelBuilder);

            ConfigureApplication(modelBuilder);
            ConfigureModule(modelBuilder);
            ConfigureTemplate(modelBuilder);
            ConfigureTemplatePrompt(modelBuilder);
            ConfigureTemplateFileConfig(modelBuilder);
            ConfigureUploadedFile(modelBuilder);
            ConfigureProcessingRequest(modelBuilder);
            ConfigureProcessedResult(modelBuilder);
            ConfigureAwsConfig(modelBuilder);
            GenAIProviderConfig(modelBuilder);
            ConfigureSystemConfig(modelBuilder);
            ConfigureUser(modelBuilder);
            ConfigureEmailSettings(modelBuilder);
            ConfigureLanguageModels(modelBuilder);

        }

        #region Global Filters (Tenant + Soft Delete)

        //private void ApplyGlobalFilters(ModelBuilder modelBuilder)
        //{
        //    foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
        //    {
        //        if (!typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
        //            continue;

        //        var parameter = Expression.Parameter(entityType.ClrType, "e");

        //        // e.IsDeleted == false
        //        var isDeletedProperty = Expression.Property(
        //            parameter,
        //            nameof(ISoftDelete.IsDeleted)
        //        );

        //        var isDeletedCondition = Expression.Equal(
        //            isDeletedProperty,
        //            Expression.Constant(false)
        //        );

        //        // e.TenantGuid == CurrentTenantGuid
        //        var tenantProperty = Expression.Property(
        //            parameter,
        //            nameof(ITenantEntity.TenantGuid)
        //        );

        //        var tenantCondition = Expression.Equal(
        //            tenantProperty,
        //            Expression.Property(
        //                Expression.Constant(this),
        //                nameof(CurrentTenantGuid)
        //            )
        //        );

        //        var filterBody = Expression.AndAlso(
        //            isDeletedCondition,
        //            tenantCondition
        //        );



        //        var lambda = Expression.Lambda(filterBody, parameter);

        //        modelBuilder.Entity(entityType.ClrType)
        //                    .HasQueryFilter(lambda);
        //    }
        //}

        private void ApplyGlobalFilters(ModelBuilder modelBuilder)
        {
            foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (!typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                    continue;

                var parameter = Expression.Parameter(entityType.ClrType, "e");

                // e.IsDeleted == false
                var isDeletedProperty = Expression.Property(
                    parameter,
                    nameof(ISoftDelete.IsDeleted)
                );

                var isDeletedCondition = Expression.Equal(
                    isDeletedProperty,
                    Expression.Constant(false)
                );

                Expression finalCondition = isDeletedCondition;

                // 👉 Apply tenant filter ONLY if entity does NOT implement IIgnoreTenantFilter
                if (!typeof(IIgnoreTenantFilter).IsAssignableFrom(entityType.ClrType))
                {
                    var tenantProperty = Expression.Property(
                        parameter,
                        nameof(ITenantEntity.TenantGuid)
                    );

                    var tenantCondition = Expression.Equal(
                        tenantProperty,
                        Expression.Property(
                            Expression.Constant(this),
                            nameof(CurrentTenantGuid)
                        )
                    );

                    finalCondition = Expression.AndAlso(
                        isDeletedCondition,
                        tenantCondition
                    );
                }

                var lambda = Expression.Lambda(finalCondition, parameter);

                modelBuilder.Entity(entityType.ClrType)
                            .HasQueryFilter(lambda);
            }
        }


        #endregion

        #region Base Defaults

        private static void ConfigureBaseDefaults(ModelBuilder modelBuilder)
        {
            ConfigureBaseEntity<Application>(modelBuilder);
            ConfigureBaseEntity<Module>(modelBuilder);
            ConfigureBaseEntity<Template>(modelBuilder);
            ConfigureBaseEntity<TemplatePrompt>(modelBuilder);
            ConfigureBaseEntity<TemplateFileConfig>(modelBuilder);
            ConfigureBaseEntity<UploadedFile>(modelBuilder);
            ConfigureBaseEntity<ProcessingRequest>(modelBuilder);
            ConfigureBaseEntity<ProcessedResult>(modelBuilder);
        }

        private static void ConfigureBaseEntity<TEntity>(ModelBuilder modelBuilder)
            where TEntity : BaseEntity
        {
            modelBuilder.Entity<TEntity>(entity =>
            {
                entity.Property(x => x.Id)
                .ValueGeneratedOnAdd();   

                entity.Property(x => x.CreatedOn)
                      .HasDefaultValueSql("SYSUTCDATETIME()");

                entity.HasIndex(x => x.TenantGuid);
                entity.HasIndex(x => x.IsDeleted);
            });
        }

        #endregion

        #region Entity Configurations

        private static void ConfigureApplication(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Application>(entity =>
            {
                entity.HasKey(x => x.ApplicationGuid);

                entity.Property(x => x.Name)
                      .HasMaxLength(200)
                      .IsRequired();

                entity.Property(x => x.Description)
                      .HasMaxLength(1000);

                entity.HasMany(x => x.Modules)
                      .WithOne()
                      .HasForeignKey(x => x.ApplicationGuid)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private static void ConfigureModule(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Module>(entity =>
            {
                entity.HasKey(x => x.ModuleGuid);

                entity.Property(x => x.Name)
                      .HasMaxLength(200)
                      .IsRequired();

                entity.Property(x => x.Description)
                      .HasMaxLength(1000);

                entity.HasMany(x => x.Templates)
                      .WithOne()
                      .HasForeignKey(x => x.ModuleGuid)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private static void ConfigureTemplate(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Template>(entity =>
            {
                entity.HasKey(x => x.TemplateGuid);

                entity.Property(x => x.Name)
                      .HasMaxLength(200)
                      .IsRequired();

                entity.Property(x => x.Description)
                      .HasMaxLength(1000);

                entity.HasOne(x => x.Prompt)
                      .WithOne()
                      .HasForeignKey<TemplatePrompt>(x => x.TemplateGuid)
                      .IsRequired(false) // 👈 optional
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.FileConfig)
                      .WithOne()
                      .HasForeignKey<TemplateFileConfig>(x => x.TemplateGuid)
                      .IsRequired(false) // 👈 optional
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
        private static void ConfigureTemplatePrompt(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TemplatePrompt>(entity =>
            {
                entity.HasKey(x => x.TemplatePromptGuid);

                entity.Property(x => x.PromptText)
                      .IsRequired();

                entity.Property(x => x.OutputSchemaJson)
                      .IsRequired();
            });
        }

        private static void ConfigureTemplateFileConfig(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TemplateFileConfig>(entity =>
            {
                entity.HasKey(x => x.TemplateFileConfigGuid);

                entity.Property(x => x.AllowedFileTypesCsv)
                      .HasMaxLength(200);
            });
        }

        private static void ConfigureUploadedFile(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UploadedFile>(entity =>
            {
                entity.HasKey(x => x.UploadedFileGuid);

                entity.Property(x => x.OriginalFileName)
                      .HasMaxLength(255)
                      .IsRequired();

                entity.Property(x => x.StoredFilePath)
                      .HasMaxLength(1000)
                      .IsRequired();

                entity.HasIndex(x => x.ReferenceNumber)
                      .IsUnique();

                entity.Ignore(x => x.SignedUrl);

            });
        }

        private static void ConfigureProcessingRequest(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProcessingRequest>(entity =>
            {
                entity.HasKey(x => x.ProcessingRequestGuid);

                entity.Property(x => x.ReferenceNumber)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(x => x.Status)
                      .HasConversion<string>();

                entity.HasIndex(x => x.ReferenceNumber)
                      .IsUnique();
            });
        }

        private static void ConfigureProcessedResult(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProcessedResult>(entity =>
            {
                entity.HasKey(x => x.ProcessedResultGuid);

                entity.Property(x => x.OutputJson)
                      .IsRequired();
            });
        }

        private static void ConfigureAwsConfig(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AwsConfig>(entity =>
            {
                entity.HasKey(x => x.AwsConfigGuid);
            });
        }

        private static void GenAIProviderConfig(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GenAIProvider>(entity =>
            {
                entity.HasKey(x => x.GenAIProviderGuid);
            });
        }

        private static void ConfigureSystemConfig(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SystemConfig>(entity =>
            {
                entity.HasKey(x => x.SystemConfigGuid);
            });
        }

        private static void ConfigureUser(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(x => x.UserGuid);

                entity.Property(x => x.UserName)
                      .HasMaxLength(200)
                      .IsRequired();

                entity.Property(x => x.Email)
                      .HasMaxLength(500);

            });
        }

        private static void ConfigureEmailSettings(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmailSetting>(entity =>
            {
                entity.HasKey(x => x.SmtpGuid);
            });
        }

        private static void ConfigureLanguageModels(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LanguageModel>(entity =>
            {
                entity.HasKey(x => x.LanguageModelGuid);
            });
        }

        
        #endregion
    }
}
