using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PersonalFinancialDashboard.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet("protected-data")]
        public IActionResult GetProtectedData()
        {
            // Only accessible with valid JWT token
            return Ok("This is protected data");
        }
    }

  
}
