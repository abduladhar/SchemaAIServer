using Microsoft.AspNetCore.Authorization;
using CommonEntities;
using Microsoft.AspNetCore.Mvc;
using SchemaAI.Entities;
using SchemaAI.ServiceContract;

namespace SchemaAI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class TemplateFileConfigsController : ControllerBase
    {
        private readonly ITemplateFileConfigService _service;

        public TemplateFileConfigsController(ITemplateFileConfigService service)
        {
            _service = service;
        }

        [HttpGet("by-template/{templateGuid:guid}")]
        [Authorize]
        public async Task<IActionResult> Get(Guid templateGuid)
        {
            var config = await _service.GetByTemplateAsync(templateGuid);
            return config == null ? NotFound() : Ok(config);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateOrUpdate(TemplateFileConfig config)
            => Ok(await _service.CreateOrUpdateAsync(config));

        [HttpPut("{templateFileConfigGuid:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateTemplateFileConfig(Guid templateFileConfigGuid, TemplateFileConfig config)
          => Ok(await _service.CreateOrUpdateAsync(config));


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
