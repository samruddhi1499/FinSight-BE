using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonalFinancialDashboard.DTOs;
using PersonalFinancialDashboard.Services.Interface;
using System.Security.Claims;

namespace PersonalFinancialDashboard.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IDasboardService _dasboardService;

        public DashboardController(IConfiguration configuration, IDasboardService dasboardService)
        {
            _configuration = configuration;
            _dasboardService = dasboardService;
        }


        [HttpGet("data")]
        public async Task<ActionResult> GetExpenseData()
        {
            string username = User.FindFirstValue(ClaimTypes.Name);

            var result = await _dasboardService.GetDashboardData(username);

            if (result == null)
            {
                return BadRequest("No Data");
            }
            return Ok(result);
        }

    }
}
