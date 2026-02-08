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
    public sealed class SystemConfigController : ControllerBase
    {
        private readonly ISystemConfigService _service;

        public SystemConfigController(ISystemConfigService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var systemConfigs = await _service.GetAllAsync();
            return systemConfigs == null ? NotFound() : Ok(systemConfigs);
        }

        [HttpGet("{systemConfigGuid:guid}")]
        [Authorize]
        public async Task<IActionResult> Get(Guid systemConfigGuid)
        {
            var app = await _service.GetAsync(systemConfigGuid);
            return app == null ? NotFound() : Ok(app);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(SystemConfig systemConfig)
        {
            var result = await _service.CreateAsync(systemConfig);
            return CreatedAtAction(nameof(Get), new { systemConfigGuid = result.SystemConfigGuid }, result);
        }

        [HttpPut("{systemConfigGuid:guid}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid systemConfigGuid, SystemConfig systemConfig)
        {
            systemConfig.SystemConfigGuid = systemConfigGuid;
            await _service.UpdateAsync(systemConfig);
            return CreatedAtAction(nameof(Get), new { systemConfigGuid = systemConfigGuid }, systemConfig);
        }

        [HttpDelete("{systemConfigGuid:guid}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid systemConfigGuid)
        {
            await _service.DeleteAsync(systemConfigGuid);
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
