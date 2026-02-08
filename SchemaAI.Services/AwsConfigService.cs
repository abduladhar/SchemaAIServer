using Amazon.Runtime.Internal.Util;
using CommonEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SchemaAI.DAL;
using SchemaAI.DALContracts;
using SchemaAI.Entities;
using SchemaAI.Persistence;
using SchemaAI.ServiceContract;

namespace SchemaAI.Services
{
    public sealed class AwsConfigService : IAwsConfigService
    {
        private readonly IAwsConfigDal _dal;
        private readonly SchemaAIDbContext _db;
        private readonly IMemoryCache _cache;
        private Guid tenantGuid = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
        private readonly ICurrentUser _currentUser;

        private static string CacheKey(Guid tenantGuid)
            => $"AWS_CONFIG_{tenantGuid}";

      

        public AwsConfigService(IAwsConfigDal dal, SchemaAIDbContext db, IMemoryCache cache, ICurrentUser currentUser)
        {
            _dal = dal;
            _db = db;
            _cache = cache;
            _currentUser = currentUser;
        }

        public async Task<AwsConfig> GetCurrentAsync()
        {
            return await _cache.GetOrCreateAsync(
                CacheKey(tenantGuid),
                async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                    entry.SlidingExpiration = TimeSpan.FromMinutes(10);

                    var configs = await this.GetAllAsync();

                    if(configs!= null && configs.Count>0)
                    {
                        return configs[0];
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            $"AWS config not found for tenant {tenantGuid}");
                    }

                    return new AwsConfig();
                })!;
        }

        public Task InvalidateCacheAsync()
        {
            _cache.Remove(CacheKey(tenantGuid));
            return Task.CompletedTask;
        }

        public async Task<List<AwsConfig>> GetAllAsync()
                        => await _dal.GetAllAsync();

       
        public async Task<AwsConfig?> GetAsync(Guid awsConfigGuid)
            => await _dal.GetByGuidAsync(awsConfigGuid);

        public async Task<AwsConfig> CreateAsync(AwsConfig awsConfig)
        {
            awsConfig.CreateSetupConfiguration(_currentUser);
            await _dal.AddAsync(awsConfig);
            await _db.SaveChangesAsync();
            return awsConfig;
        }

        public async Task UpdateAsync(AwsConfig awsConfig)
        {
            awsConfig.CreateSetupConfiguration(_currentUser);
            await _dal.UpdateAsync(awsConfig);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid aWSConfigGuid)
        {
            var entity = await _dal.GetByGuidAsync(aWSConfigGuid);
            if (entity == null) return;

            await _dal.SoftDeleteAsync(entity);
            await _db.SaveChangesAsync();
        }

        #region Paging

        public async Task<PagedResult<AwsConfig>> GetPagedAsync(
        int pageNumber,
        int pageSize)
        {
            return await _dal.GetPagedAsync(pageNumber, pageSize);
        }
    
        public async Task<List<AwsConfig>> GetScrollAsync(
        int lastId,
        int batchSize)
        {
            return await _dal.GetScrollAsync(lastId, batchSize);
        }

        #endregion
    }
}
