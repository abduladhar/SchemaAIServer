using Microsoft.AspNetCore.Authorization;
using CommonEntities;
using Microsoft.AspNetCore.Mvc;
using SchemaAI.Entities;
using SchemaAI.ServiceContract;

namespace SchemaAI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class ProcessedResultsController : ControllerBase
    {
        private readonly IProcessedResultService _service;

        public ProcessedResultsController(IProcessedResultService service)
        {
            _service = service;
        }

        [HttpGet("by-request/{processingRequestGuid:guid}")]
        [Authorize]
        public async Task<IActionResult> Get(Guid processingRequestGuid)
        {
            var result = await _service.GetByRequestAsync(processingRequestGuid);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet("by-processedid/{processedRequestGuid:guid}")]
        [Authorize]
        public async Task<IActionResult> GetByProcessedGuidAsync(Guid processedRequestGuid)
        {
            var result = await _service.GetByProcessedGuidAsync(processedRequestGuid);
            return result == null ? NotFound() : Ok(result);
        }

        //[HttpGet("ByReference/{referanceNo}")]
        //public async Task<IActionResult> GetByReferanceNoAsync(string referanceNo)
        //{
        //    var result = await _service.GetByReferanceNoAsync(referanceNo);
        //    return result == null ? NotFound() : Ok(result);
        //}

        [HttpGet("ByReference/{referenceNo}")]
        public async Task<IActionResult> GetByReferenceNoAsync(
        string referenceNo,
        [FromHeader(Name = "api-key")] string apiKey)
        {
            var result = await _service.GetByReferenceNoAsync(referenceNo, apiKey);
            return result == null ? NotFound() : Ok(result);
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var modules = await _service.GetAllAsync();
            return Ok(modules);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(ProcessedResult result)
            => Ok(await _service.CreateAsync(result));


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
