using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalFinancialDashboard.Entities;
using PersonalFinancialDashboard.Models;

using System.Security.Claims;
using static Azure.Core.HttpHeader;

namespace PersonalFinancialDashboard.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserDetailsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public UserDetailsController(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }


        [HttpPost("profile-data")]
        public async Task<ActionResult<UserDetails>> SetProfileData(UserDetailsDto request)
        {

            string username = User.FindFirstValue(ClaimTypes.Name);
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);

            if (user != null) {
                var userDetails = new UserDetails
                {
                    CurrentBalance = request.CurrentBalance,
                    SalaryPerMonth = request.SalaryPerMonth,
                    UserId = user.Id, // Set FK directly

                };

                _context.UserDetails.Add(userDetails);
                await _context.SaveChangesAsync();

                return Ok(userDetails);
            }

            return BadRequest("No user");
           
        }

        [HttpPut("update-profile-data")]
        public async Task<ActionResult> UpdateProfileData(UserDetailsDto request)
        {

            string username = User.FindFirstValue(ClaimTypes.Name);
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);

            if (user != null)
            {
                var userDetails = await _context.UserDetails.SingleOrDefaultAsync(u => u.UserId == user.Id);

                userDetails.SalaryPerMonth = request.SalaryPerMonth;
                userDetails.CurrentBalance = request.CurrentBalance;

                
                await _context.SaveChangesAsync();

                return Ok(new {userDetails.SalaryPerMonth, userDetails.CurrentBalance, userDetails.Id,userDetails.UserId});
            }

            return BadRequest("No user");

        }

        [HttpGet("profile")]
        public async Task<ActionResult> GetProfileData()
        {
            string username = User.FindFirstValue(ClaimTypes.Name);
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);

            if (!string.IsNullOrEmpty(username)) {
                var details = await _context.UserDetails.SingleOrDefaultAsync(u => u.UserId == user.Id);
                if (details != null) {

                    return Ok(new { details.SalaryPerMonth, details.CurrentBalance, details.Id, details.UserId, username });
                }
                return BadRequest("Complete ur profile");
                
            }

            return BadRequest("User does not exist");
        }

        [HttpPost("add-categories")]
        public async Task<ActionResult> SetRecurringCategories(RecurringCategoriesDto request)
        {
            string username = User.FindFirstValue(ClaimTypes.Name);
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return BadRequest("No user");

            var expenseCategory = await _context.ExpenseCategories
                .FirstOrDefaultAsync(c => c.Category == request.Category);

            if (expenseCategory == null)
                return BadRequest("Invalid category");

            var recurringCategory = new RecurringCategories
            {
                ExpenseCategoriesId = expenseCategory.Id,
                UserId = user.Id,
                CapAmount = request.capAmount,
                User = user,
                ExpenseCategories = expenseCategory
            };

            _context.RecurringCategories.Add(recurringCategory);
            await _context.SaveChangesAsync();

            return Ok("Added");
        }

        [HttpPut("update-categories")]
        public async Task<ActionResult> UpdateCapAmount(RecurringCategoriesDto request)
        {
            string username = User.FindFirstValue(ClaimTypes.Name);
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (user == null)
                return BadRequest("No user");

            var expenseCategory = await _context.ExpenseCategories
                .FirstOrDefaultAsync(c => c.Category == request.Category);
            if (expenseCategory == null)
                return BadRequest("Invalid category");

            var recurringCategory = await _context.RecurringCategories
                .FirstOrDefaultAsync(c => c.UserId == user.Id && c.ExpenseCategoriesId == expenseCategory.Id);

            if (recurringCategory == null)
                return NotFound("Recurring category not found");

            recurringCategory.CapAmount = request.capAmount;
            await _context.SaveChangesAsync();

            return Ok("Updated");
        }





        [HttpGet("selected-categories")]
        public async Task<ActionResult> GetCategories()
        {
            string username = User.FindFirstValue(ClaimTypes.Name);
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
            var resultList = new List<object>();

            if (!string.IsNullOrEmpty(username))
            {
                var categories = await _context.RecurringCategories
                                .Where(u => u.UserId == user.Id)
                                .ToListAsync();

                if (categories != null)
                {
                    foreach (var cat in categories)
                    {
                        var c = await _context.ExpenseCategories.FindAsync(cat.ExpenseCategoriesId);
                        resultList.Add(new { CategoryId = cat.ExpenseCategoriesId, CategoryName = c.Category, CapAmount = cat.CapAmount }); ;
                    }

                    return Ok(resultList);
                }
                return BadRequest("Add Recurring categories");

            }

            return BadRequest("User does not exist");
        }

        [HttpGet("remaining-categories")]
        public async Task<ActionResult> GetRemainingCategories()
        {
            string username = User.FindFirstValue(ClaimTypes.Name);
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return BadRequest("User does not exist");

            // Get IDs of categories user already has
            var userCategoryIds = await _context.RecurringCategories
                .Where(rc => rc.UserId == user.Id)
                .Select(rc => rc.ExpenseCategoriesId)
                .ToListAsync();

            // Get all expense categories NOT in user's list
            var remainingCategories = await _context.ExpenseCategories
                .Where(ec => !userCategoryIds.Contains(ec.Id))
                .Select(ec => ec.Category) // assuming Category is the name string
                .ToListAsync();

            if (remainingCategories.Count == 0)
                return BadRequest("Add Recurring categories");

            return Ok(remainingCategories);
        }

        [HttpDelete("delete-categories/{id}")]
        public async Task<ActionResult> DeleteCategories(int id)
        {
            string username = User.FindFirstValue(ClaimTypes.Name);
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);


            if (user != null)
            {
              var cat = await _context.RecurringCategories
                        .SingleOrDefaultAsync(u => u.UserId == user.Id && u.ExpenseCategoriesId == id);

                    _context.RecurringCategories.Remove(cat);
                    await _context.SaveChangesAsync();


                

                return NoContent();


            }

            return BadRequest("No user");
        }
    }
}
