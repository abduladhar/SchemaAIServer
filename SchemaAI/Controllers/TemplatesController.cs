using Microsoft.AspNetCore.Authorization;
using CommonEntities;
using Microsoft.AspNetCore.Mvc;
using SchemaAI.Entities;
using SchemaAI.ServiceContract;

namespace SchemaAI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class TemplatesController : ControllerBase
    {
        private readonly ITemplateService _service;

        public TemplatesController(ITemplateService service)
        {
            _service = service;
        }

        [HttpGet("{templateGuid:guid}")]
        [Authorize]
        public async Task<IActionResult> Get(Guid templateGuid)
        {
            var template = await _service.GetFullAsync(templateGuid);
            return template == null ? NotFound() : Ok(template);
        }

        // GET: api/templates
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var templates = await _service.GetAllAsync();
            return Ok(templates);
        }

        [HttpGet("by-module/{moduleGuid:guid}")]
        [Authorize]
        public async Task<IActionResult> GetByModule(Guid moduleGuid)
            => Ok(await _service.GetByModuleAsync(moduleGuid));

        [HttpPost]
        public async Task<IActionResult> Create(Template template)
            => Ok(await _service.CreateAsync(template));

        [HttpPut("{templateGuid:guid}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid templateGuid, Template template)
        {
            template.TemplateGuid = templateGuid;
            await _service.UpdateAsync(template);
            return NoContent();
        }

        [HttpDelete("{templateGuid:guid}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid templateGuid)
        {
            await _service.DeleteAsync(templateGuid);
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
