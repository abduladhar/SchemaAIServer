using CommonEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchemaAI.Entities;
using SchemaAI.ServiceContract;

namespace SchemaAI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class ApplicationsController : ControllerBase
    {
        private readonly IApplicationService _service;

        public ApplicationsController(IApplicationService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{applicationGuid:guid}")]
        public async Task<IActionResult> Get(Guid applicationGuid)
        {
            var app = await _service.GetAsync(applicationGuid);
            return app == null ? NotFound() : Ok(app);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(Application application)
        {
            var result = await _service.CreateAsync(application);
            return CreatedAtAction(nameof(Get), new { applicationGuid = result.ApplicationGuid }, result);
        }

        [HttpPut("{applicationGuid:guid}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid applicationGuid, Application application)
        {
            application.ApplicationGuid = applicationGuid;
            await _service.UpdateAsync(application);
            return CreatedAtAction(nameof(Get), new { applicationGuid = applicationGuid }, application);
        }

        [HttpDelete("{applicationGuid:guid}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid applicationGuid)
        {
            await _service.DeleteAsync(applicationGuid);
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
