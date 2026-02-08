using Microsoft.AspNetCore.Authorization;
using CommonEntities;
using Microsoft.AspNetCore.Mvc;
using SchemaAI.Entities;
using SchemaAI.ServiceContract;

namespace SchemaAI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class ProcessingRequestsController : ControllerBase
    {
        private readonly IProcessingRequestService _service;

        public ProcessingRequestsController(IProcessingRequestService service)
        {
            _service = service;
        }

        [HttpGet("{referenceNumber}")]
        [Authorize]
        public async Task<IActionResult> Get(string referenceNumber)
        {
            var request = await _service.GetByReferenceAsync(referenceNumber);
            return request == null ? NotFound() : Ok(request);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(ProcessingRequest request)
            => Ok(await _service.CreateAsync(request));

        [HttpPut("{requestGuid:guid}/status")]
        [Authorize]
        public async Task<IActionResult> UpdateStatus(Guid requestGuid, string status)
        {
            await _service.UpdateStatusAsync(requestGuid, status);
            return NoContent();
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var modules = await _service.GetAllAsync();
            return Ok(modules);
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
