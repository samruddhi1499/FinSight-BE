using Microsoft.EntityFrameworkCore;
using PersonalFinancialDashboard.DTOs;
using PersonalFinancialDashboard.Entities;
using PersonalFinancialDashboard.Services.Interface;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Security.Claims;

namespace PersonalFinancialDashboard.Services
{
    public class ExpenseServiceImpl : IExpenseService
    {

        private readonly AppDbContext _context;

        public ExpenseServiceImpl(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> AddExpenseAsync(ExpenseDto request, string username)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (user == null) return "No user";

            var exists = await _context.RecurringCategories.AnyAsync(
                re => re.ExpenseCategoriesId == request.ExpenseCategoriesId && re.UserId == user.Id);
            if (!exists) return "Add to Recurring Categories first";

            var expense = new Expense
            {
                ExpenseCategoriesId = request.ExpenseCategoriesId,
                Amount = request.Amount,
                ExpenseDate = request.ExpenseDate,
                UserId = user.Id,
            };

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();
            return "Received";
        }

        public async Task<List<ExpenseSummaryDto>> GetExpensesAsync(string username, bool toBeFiltered)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);

            if (user != null)
            {
                // LINQ join as per your SQL
                var result = await (from ec in _context.ExpenseCategories
                                    join e in _context.Expenses on ec.Id equals e.ExpenseCategoriesId
                                    join rc in _context.RecurringCategories on e.ExpenseCategoriesId equals rc.ExpenseCategoriesId
                                    where e.UserId == user.Id
                                    group new { e, rc } by new
                                    {
                                        ec.Id,
                                        ec.Category,
                                        ExpenseYear = e.ExpenseDate.Year,
                                        ExpenseMonth = e.ExpenseDate.Month,
                                        rc.CapAmount
                                    } into g
                                    select new ExpenseSummaryDto
                                    {
                                        Id = g.Key.Id,
                                        Category = g.Key.Category,
                                        ExpenseYear = g.Key.ExpenseYear,
                                        ExpenseMonth = g.Key.ExpenseMonth,
                                        CapAmount = g.Key.CapAmount,
                                        TotalAmount = g.Sum(x => x.e.Amount),
                                        IsOverBudget = g.Sum(x => x.e.Amount) <= g.Key.CapAmount ? "Under" : "Over"
                                    }).ToListAsync();

                // If you want to filter by the current month in C#, you can add:
                if (toBeFiltered)
                {
                    DateTime now = DateTime.Now;
                    var filtered = result.Where(x => x.ExpenseYear == now.Year && x.ExpenseMonth == now.Month).ToList();

                    return filtered;
                }

                return result;
            }

            return new List<ExpenseSummaryDto>();


        }

        public async Task<List<ExpenseDetailDto>> GetExpenseByCategoryAsync(int categoryId, string? username)
        {
            DateTime now = DateTime.Now;
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);

            var expenses = await (from e in _context.Expenses
                                  join ec in _context.ExpenseCategories on e.ExpenseCategoriesId equals ec.Id
                                  where e.ExpenseCategoriesId == categoryId
                                        && e.UserId == user.Id
                                        && e.ExpenseDate.Month == now.Month
                                        && e.ExpenseDate.Year == now.Year
                                  select new ExpenseDetailDto
                                  {
                                      Id = e.Id,
                                      Category = ec.Category,
                                      Amount = e.Amount,
                                      ExpenseDate = e.ExpenseDate
                                  })
                     .ToListAsync();
            if (expenses != null)
            {
                return expenses;
            }

            return new List<ExpenseDetailDto>();
        }

        public async Task<bool> UpdateExpenseAmountAsync(int expenseId, float amount)
        {
            var expense = await _context.Expenses.FindAsync(expenseId);

            if (expense != null)
            {

                expense.Amount = amount;

                await _context.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<bool> DeleteExpenseAsync(int expenseId)
        {
            var expense = await _context.Expenses.FindAsync(expenseId);

            if (expense != null)
            {

                _context.Expenses.Remove(expense);

                await _context.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<bool> MarkComplete(string username)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
            DateTime now = DateTime.Now;

            var complete = await _context.MarkCompletes.SingleOrDefaultAsync(u => u.UserId == user.Id && u.Year == now.Year && u.Month == now.Month);

            if (complete == null)
            {

               

                var totalCap = await _context.RecurringCategories
                              .Where(rc => rc.UserId == user.Id)
                              .SumAsync(rc => rc.CapAmount);

                var totalExpense = await _context.Expenses
                    .Where(e => e.UserId == user.Id && e.ExpenseDate.Year == now.Year)
                    .SumAsync(e => e.Amount);

                var difference = totalCap - totalExpense;

                var userDetails = await _context.UserDetails.SingleOrDefaultAsync(u => u.UserId == user.Id);

                if (userDetails != null)
                {

                    userDetails.CurrentBalance += difference;
                    await _context.SaveChangesAsync();

                    var markComplete = new MarkComplete
                    {
                        Year = now.Year,
                        Month = now.Month,
                        UserId = user.Id,
                        Savings = difference
                    };

                    _context.MarkCompletes.Add(markComplete);
                    await _context.SaveChangesAsync();


                    return true;

                }

            }

            return false;

        }

        public async Task<bool> CheckComplete(string username)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
            DateTime now = DateTime.Now;

            var complete = await _context.MarkCompletes.SingleOrDefaultAsync(u => u.UserId == user.Id && u.Year == now.Year && u.Month == now.Month);

            if (complete != null)
                return true;
            return false;
        }


    }
}
