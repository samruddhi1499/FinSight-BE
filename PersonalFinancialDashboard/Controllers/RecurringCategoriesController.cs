
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonalFinancialDashboard.DTOs;
using PersonalFinancialDashboard.Entities;
using PersonalFinancialDashboard.Services.Interface;
using System.Security.Claims;

namespace PersonalFinancialDashboard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RecurringCategoriesController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        private readonly IRecurringCategoriesService _recurringCategoriesService;

        public RecurringCategoriesController(IConfiguration configuration, IRecurringCategoriesService recurringCategoriesService)
        {
            _configuration = configuration;
            _recurringCategoriesService = recurringCategoriesService;
        }

        [HttpPost("add-categories")]
        public async Task<ActionResult> SetRecurringCategories(RecurringCategoriesDto request)
        {
            string username = User.FindFirstValue(ClaimTypes.Name);
            var result = await _recurringCategoriesService.AddRecurringCategoryAsync(request, username);

            if (result.Val)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);

        }

        [HttpPut("update-categories")]
        public async Task<ActionResult> UpdateCapAmount(RecurringCategoriesDto request)
        {
            string username = User.FindFirstValue(ClaimTypes.Name);
            var result = await _recurringCategoriesService.UpdateRecurringCategoryAsync(request, username);

            if (result.Val)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);

        }





        [HttpGet("selected-categories")]
        public async Task<ActionResult> GetCategories()
        {
            string username = User.FindFirstValue(ClaimTypes.Name);
            var result = await _recurringCategoriesService.GetSelectedCategoriesAsync(username);

            if(result == null)
            {
                return Unauthorized("User does not exist");
            }
            else if(result.Count() == 0)
            {
                return BadRequest("Add Recurring categories");
            }
            else
            {
                return Ok(result);
            }

        }

        [HttpGet("remaining-categories")]
        public async Task<ActionResult> GetRemainingCategories()
        {
            string username = User.FindFirstValue(ClaimTypes.Name);
            var result = await _recurringCategoriesService.GetRemainingCategoriesAsync(username);

            if (result == null)
            {
                return Unauthorized("User does not exist");
            }
            else if (result.Count() == 0)
            {
                return BadRequest("No Remaining categories");
            }
            else
            {
                return Ok(result);
            }
        }

        [HttpDelete("delete-categories/{id}")]
        public async Task<ActionResult> DeleteCategories(int id)
        {
            string username = User.FindFirstValue(ClaimTypes.Name);
            var result = await _recurringCategoriesService.DeleteRecurringCategoryAsync(id, username);

            if (result)
                return NoContent();
            return BadRequest("No user");

        
        }
    }
}
