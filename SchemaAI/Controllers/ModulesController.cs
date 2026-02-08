using CommonEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchemaAI.Entities;
using SchemaAI.ServiceContract;

namespace SchemaAI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class ModulesController : ControllerBase
    {
        private readonly IModuleService _service;

        public ModulesController(IModuleService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        // GET: api/modules/by-application/{applicationGuid}
        [HttpGet("by-application/{applicationGuid:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize]
        public async Task<IActionResult> GetByApplication(Guid applicationGuid)
        {
            var modules = await _service.GetByApplicationAsync(applicationGuid);
            return Ok(modules);
        }

        [HttpGet("by-module/{moduleguid:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize]
        public async Task<IActionResult> GetByModule(Guid moduleguid)
        {
            var modules = await _service.GetAsync(moduleguid);
            return Ok(modules);
        }

        // GET: api/modules
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var modules = await _service.GetAllAsync();
            return Ok(modules);
        }

        // POST: api/modules
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] Module module)
        {
            if (module == null)
                return BadRequest("Module payload is required.");

            var createdModule = await _service.CreateAsync(module);

            return CreatedAtAction(
                nameof(GetByModule),
                new { moduleGuid = createdModule.ModuleGuid },
                createdModule
            );
        }

        // PUT: api/modules/{moduleGuid}
        [HttpPut("{moduleGuid:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<IActionResult> Update(Guid moduleGuid, [FromBody] Module module)
        {
            if (module == null)
                return BadRequest("Module payload is required.");

            if (module.ModuleGuid != Guid.Empty && module.ModuleGuid != moduleGuid)
                return BadRequest("ModuleGuid in route and body do not match.");

            module.ModuleGuid = moduleGuid;

            await _service.UpdateAsync(module);

            return CreatedAtAction(
               nameof(GetByModule),
               new { moduleGuid = moduleGuid },
               module
           );
         
        }

        // DELETE: api/modules/{moduleGuid}
        [HttpDelete("{moduleGuid:guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(Guid moduleGuid)
        {
            await _service.DeleteAsync(moduleGuid);
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
