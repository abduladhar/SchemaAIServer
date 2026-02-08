using Microsoft.AspNetCore.Authorization;
using CommonEntities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchemaAI.DAL;
using SchemaAI.Entities;
using SchemaAI.ServiceContract;

namespace SchemaAI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class AwsConfigController : ControllerBase
    {
        private readonly IAwsConfigService _service;

        public AwsConfigController(IAwsConfigService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var awsConfigs = await _service.GetAllAsync();
            var awsConfigsList = awsConfigs.Select(x => new AwsConfig { AwsConfigGuid = x.AwsConfigGuid, AccessKeyID = string.IsNullOrEmpty(x.AccessKeyID) || x.AccessKeyID.Length <= 4 ? "XXXX" : x.AccessKeyID.Substring(0, 4) + "XXXX", SecretAccessKey = string.IsNullOrEmpty(x.SecretAccessKey) || x.SecretAccessKey.Length <= 4 ? "XXXX" : x.SecretAccessKey.Substring(0, 4) + "XXXX", Region = x.Region, S3BucketName = x.S3BucketName, IsCurrentItem = x.IsCurrentItem }).ToList();
            return awsConfigsList == null ? NotFound() : Ok(awsConfigsList);
        }

        [HttpGet("{awsConfigGuid:guid}")]
        [Authorize]
        public async Task<IActionResult> Get(Guid awsConfigGuid)
        {
            var app = await _service.GetAsync(awsConfigGuid);
            return app == null ? NotFound() : Ok(app);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(AwsConfig awsConfig)
        {
            var result = await _service.CreateAsync(awsConfig);
            return CreatedAtAction(nameof(Get), new { awsConfigGuid = result.AwsConfigGuid }, result);
        }

        [HttpPut("{awsConfigGuid:guid}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid awsConfigGuid, AwsConfig awsConfig)
        {
            awsConfig.AwsConfigGuid = awsConfigGuid;
            await _service.UpdateAsync(awsConfig);
            return CreatedAtAction(nameof(Get), new { awsConfigGuid = awsConfigGuid }, awsConfig);
        }

        [HttpDelete("{awsConfigGuid:guid}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid awsConfigGuid)
        {
            await _service.DeleteAsync(awsConfigGuid);
            return NoContent();
        }


        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged(
        [FromQuery] PagingRequest request)
        {
            var result = await _service.GetPagedAsync(
                request.PageNumber,
                request.PageSize);

            return Ok(result);
        }

        [HttpGet("scrollPaged")]
        public async Task<IActionResult> GetScrollAsync(
        [FromQuery] ScrolledPagingRequest request)
        {
            var result = await _service.GetScrollAsync(
                request.LastId,
                request.PageSize);

            return Ok(result);
        }
    }
}
