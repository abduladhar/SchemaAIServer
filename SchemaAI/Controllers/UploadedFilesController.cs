using Microsoft.AspNetCore.Authorization;
using CommonEntities;
using Microsoft.AspNetCore.Mvc;
using SchemaAI.Entities;
using SchemaAI.ServiceContract;

namespace SchemaAI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class UploadedFilesController : ControllerBase
    {
        private readonly IUploadedFileService _service;
        private readonly IFileDataProcessorFactory _factory;
        private readonly ISystemConfigService _systemConfigService;
        public UploadedFilesController(IUploadedFileService service, 
            IFileDataProcessorFactory factory, 
            ISystemConfigService systemConfigService)
        {
            _service = service;
            _factory = factory;
            _systemConfigService = systemConfigService;
        }

        [HttpGet("{referenceNumber}")]
        [Authorize]
        public async Task<IActionResult> Get(string referenceNumber)
        {
            var file = await _service.GetByReferenceAsync(referenceNumber);
            return file == null ? NotFound() : Ok(file);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(UploadedFile file)
            => Ok(await _service.CreateAsync(file));


        [HttpPost("GenerateAIObjects/{uploadedFileGuid:guid}")]
        [Authorize]
        public async Task<IActionResult> GenerateAIObjects(Guid uploadedFileGuid)
        {
            bool bResult = false;
            var objSystemConfig = await _systemConfigService.GetAllAsync();
            if(objSystemConfig!=null && objSystemConfig.Count>0)
            {
                var processorService = _factory.Create(objSystemConfig[0]);
                bResult = await processorService.ProcessAsync(uploadedFileGuid);
            }
            return Ok(bResult);
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
