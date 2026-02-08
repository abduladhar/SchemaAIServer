using Microsoft.AspNetCore.Authorization;
using CommonEntities;
using Microsoft.AspNetCore.Mvc;
using SchemaAI.Entities;
using SchemaAI.ServiceContract;

namespace SchemaAI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class TemplatePromptsController : ControllerBase
    {
        private readonly ITemplatePromptService _service;

        public TemplatePromptsController(ITemplatePromptService service)
        {
            _service = service;
        }

        [HttpGet("by-template/{templateGuid:guid}")]
        [Authorize]
        public async Task<IActionResult> Get(Guid templateGuid)
        {
            var prompt = await _service.GetByTemplateAsync(templateGuid);
            return prompt == null ? NotFound() : Ok(prompt);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateOrUpdate(TemplatePrompt prompt)
            => Ok(await _service.CreateOrUpdateAsync(prompt));

        [HttpPut("{templatePromptGuid:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateTemplatePrompt(Guid templatePromptGuid, TemplatePrompt prompt)
            => Ok(await _service.CreateOrUpdateAsync(prompt));


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
