using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchemaAI.DAL;
using SchemaAI.Entities;
using SchemaAI.ServiceContract;
using SchemaAI.Services;

namespace SchemaAI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class LanguageModelsController : ControllerBase
    {
        private readonly ILanguageModelService _service;

        public LanguageModelsController(ILanguageModelService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var LanguageModels = await _service.GetAllAsync();
            return  Ok(LanguageModels);
        }

        [HttpGet("{LanguageModelGuid:guid}")]
        [Authorize]
        public async Task<IActionResult> Get(Guid LanguageModelGuid)
        {
            var app = await _service.GetAsync(LanguageModelGuid);
            return app == null ? NotFound() : Ok(app);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(LanguageModel LanguageModel)
        {
            var result = await _service.CreateAsync(LanguageModel);
            return CreatedAtAction(nameof(Get), new { LanguageModelGuid = result.LanguageModelGuid }, result);
        }

        [HttpPut("{LanguageModelGuid:guid}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid LanguageModelGuid, LanguageModel LanguageModel)
        {
            LanguageModel.LanguageModelGuid = LanguageModelGuid;
            await _service.UpdateAsync(LanguageModel);
            return CreatedAtAction(nameof(Get), new { LanguageModelGuid = LanguageModelGuid }, LanguageModel);
        }

        [HttpDelete("{LanguageModelGuid:guid}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid LanguageModelGuid)
        {
            await _service.DeleteAsync(LanguageModelGuid);
            return NoContent();
        }

        [HttpGet("by-provider/{provider}")]
        [Authorize]
        public async Task<IActionResult> GetByProvider(string provider)
        {
            if (string.IsNullOrWhiteSpace(provider))
                return BadRequest("Provider is required.");

            // Normalize (optional)
            provider = provider.Trim();

            // Fetch from DB / service
            var models = await _service.GetAllLanguageModelAsync(provider);

            if (models == null || !models.Any())
                return NotFound(new { message = $"No models found for provider '{provider}'" });

            return Ok(models);
        }
    }
}
