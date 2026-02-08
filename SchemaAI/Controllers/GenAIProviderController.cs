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
    public sealed class GenAIProviderController : ControllerBase
    {
        private readonly IGenAIProviderService _service;

        public GenAIProviderController(IGenAIProviderService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var genAIProviders = await _service.GetAllAsync();
            var genAIProvidersList = genAIProviders.Select(x => new GenAIProvider { GenAIProviderGuid = x.GenAIProviderGuid, SecretAccessKey = string.IsNullOrEmpty(x.SecretAccessKey) || x.SecretAccessKey.Length <= 4 ? "XXXX" : x.SecretAccessKey.Substring(0, 4) + "XXXX", BaseUrl = x.BaseUrl, ModelName = x.ModelName, Name = x.Name }).ToList();
            return genAIProvidersList == null ? NotFound() : Ok(genAIProvidersList);
        }

        [HttpGet("{genAIProviderGuid:guid}")]
        [Authorize]
        public async Task<IActionResult> Get(Guid genAIProviderGuid)
        {
            var app = await _service.GetAsync(genAIProviderGuid);
            return app == null ? NotFound() : Ok(app);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(GenAIProvider genAIProvider)
        {
            var result = await _service.CreateAsync(genAIProvider);
            return CreatedAtAction(nameof(Get), new { genAIProviderGuid = result.GenAIProviderGuid }, result);
        }

        [HttpPut("{genAIProviderGuid:guid}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid genAIProviderGuid, GenAIProvider genAIProvider)
        {
            genAIProvider.GenAIProviderGuid = genAIProviderGuid;
            await _service.UpdateAsync(genAIProvider);
            return CreatedAtAction(nameof(Get), new { genAIProviderGuid = genAIProviderGuid }, genAIProvider);
        }

        [HttpDelete("{genAIProviderGuid:guid}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid genAIProviderGuid)
        {
            await _service.DeleteAsync(genAIProviderGuid);
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
