using Microsoft.EntityFrameworkCore;
using PersonalFinancialDashboard.DTOs;
using PersonalFinancialDashboard.Entities;
using PersonalFinancialDashboard.Services.Interface;

namespace PersonalFinancialDashboard.Services
{
    public class RecurringCategoriesServiceImpl : IRecurringCategoriesService
    {

        private readonly AppDbContext _context;

        public RecurringCategoriesServiceImpl(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RecurringCatMessageDto> AddRecurringCategoryAsync(RecurringCategoriesDto request, string username)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return new RecurringCatMessageDto("No user", false);

            var expenseCategory = await _context.ExpenseCategories
                .FirstOrDefaultAsync(c => c.Category == request.Category);

            if (expenseCategory == null)
                return new RecurringCatMessageDto("Invalid category", false);

            var recurringCategory = new RecurringCategories
            {
                ExpenseCategoriesId = expenseCategory.Id,
                UserId = user.Id,
                CapAmount = Math.Round(request.CapAmount,2),
                User = user,
                ExpenseCategories = expenseCategory
            };

            _context.RecurringCategories.Add(recurringCategory);
            await _context.SaveChangesAsync();

            return new RecurringCatMessageDto("Added", true);
        }

        public async Task<RecurringCatMessageDto> UpdateRecurringCategoryAsync(RecurringCategoriesDto request, string username)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (user == null)
                return new RecurringCatMessageDto("No user", false);

            var expenseCategory = await _context.ExpenseCategories
                .FirstOrDefaultAsync(c => c.Category == request.Category);
            if (expenseCategory == null)
                return new RecurringCatMessageDto("Invalid category", false);

            var recurringCategory = await _context.RecurringCategories
                .FirstOrDefaultAsync(c => c.UserId == user.Id && c.ExpenseCategoriesId == expenseCategory.Id);

            if (recurringCategory == null)
                return new RecurringCatMessageDto("Recurring category not found", false);

            recurringCategory.CapAmount = Math.Round(request.CapAmount, 2);
            await _context.SaveChangesAsync();

            return new RecurringCatMessageDto("Updated", true);
        }

        public async Task<List<RecurringCategoriesDto>?> GetSelectedCategoriesAsync(string username)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
            var resultList = new List<RecurringCategoriesDto>();

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
                        resultList.Add(new RecurringCategoriesDto{ CategoryId = cat.ExpenseCategoriesId, Category = c.Category, CapAmount = cat.CapAmount }); ;
                    }

                    return (resultList);
                }
                return (resultList);

            }

            return null;
        }

        public async Task<List<string>?> GetRemainingCategoriesAsync(string username)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return null;

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

            return (remainingCategories);
        }

        public async Task<bool> DeleteRecurringCategoryAsync(int catId, string username)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);


            if (user != null)
            {
                var cat = await _context.RecurringCategories
                          .SingleOrDefaultAsync(u => u.UserId == user.Id && u.ExpenseCategoriesId == catId);

                _context.RecurringCategories.Remove(cat);
                await _context.SaveChangesAsync();

                return true;
            }

            return false;
        }
    }
}
