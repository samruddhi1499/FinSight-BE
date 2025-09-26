using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinancialDashboard.DTOs;
using PersonalFinancialDashboard.Services.Interface;
using System.Security.Claims;

namespace PersonalFinancialDashboard.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IExpenseService _expenseService;

        public ExpenseController(IConfiguration configuration, IExpenseService expenseService)
        {
            _configuration = configuration;
            _expenseService = expenseService;
        }


        [HttpPost("add-expense")]
        public async Task<ActionResult> AddExpenseData(ExpenseDto request)
        {

            string username = User.FindFirstValue(ClaimTypes.Name);

            string result = await _expenseService.AddExpenseAsync(request, username);
            if (result == "Received")
                return Ok(result);
            return BadRequest(result);

        }

        [HttpGet("expense")]
        public async Task<ActionResult> GetExpenseData()
        {
            string username = User.FindFirstValue(ClaimTypes.Name);
         
            List<ExpenseSummaryDto> result = await _expenseService.GetExpensesAsync(username, true);

            if(result.Count == 0)
            {
                return BadRequest("No Data");
            }
            return Ok(result);
        }

        [HttpGet("get-expense/{id}")]
        public async Task<ActionResult> GetParticularExpenseData(int id)
        {
            
            string username = User.FindFirstValue(ClaimTypes.Name);

            List<ExpenseDetailDto> result = await _expenseService.GetExpenseByCategoryAsync(id, username);

            if (result.Count == 0)
            {
                return BadRequest("No Data");
            }
            return Ok(result);

        }

        [HttpPut("update-expense/{id}")]
        public async Task<ActionResult> UpdateExpenseData(int id, [FromBody] float amount)
        { 

            bool result = await _expenseService.UpdateExpenseAmountAsync(id,amount);

            if (!result)
            {
                return BadRequest("Invalid Transaction");
            }
            return Ok(result);

        }

        [HttpDelete("delete-expense/{id}")]
        public async Task<ActionResult> DeleteExpenseData(int id)
        {
            bool result = await _expenseService.DeleteExpenseAsync(id);

            if (!result)
            {
                return BadRequest("Invalid Transaction");
            }
            return Ok("deleted");
        }


        [HttpGet("expense-history")]
        public async Task<ActionResult> GetExpenseHistoryData()
        {
            string username = User.FindFirstValue(ClaimTypes.Name);

            List<ExpenseSummaryDto> result = await _expenseService.GetExpensesAsync(username, false);

            if (result.Count == 0)
            {
                return BadRequest("No Data");
            }
            return Ok(result);
        }


        [HttpPut("mark-complete")]
        public async Task<ActionResult> MarkAsComplete()
        {
            string username = User.FindFirstValue(ClaimTypes.Name);
            bool result = await _expenseService.MarkComplete(username);

            if (!result)
            {
                return BadRequest("Invalid Transaction");
            }
            return Ok("Marked");

        }

        [HttpGet("check-complete")]
        public async Task<ActionResult> CheckComplete()
        {
            string username = User.FindFirstValue(ClaimTypes.Name);
            bool result = await _expenseService.CheckComplete(username);

            if (!result)
            {
                return Ok("No");
            }
            return Ok("Yes");

        }

    }
}
