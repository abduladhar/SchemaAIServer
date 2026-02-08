using Microsoft.AspNetCore.Mvc;

namespace SchemaAI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                status = "Healthy",
                service = "YourApp API",
                timestamp = DateTime.UtcNow
            });
        }
    }
}
