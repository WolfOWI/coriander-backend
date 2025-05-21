using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoriCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
    /// <summary>
    /// Checks the health of the backend service
    /// </summary>
    /// <returns>The health of the service</returns>
    [HttpGet]
    public IActionResult Check()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }
    }
}
