using Microsoft.EntityFrameworkCore;
using PersonalFinancialDashboard.DTOs;
using PersonalFinancialDashboard.Entities;
using PersonalFinancialDashboard.Services.Interface;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;

namespace PersonalFinancialDashboard.Services
{
    public class DashboardServiceImpl : IDasboardService
    {
      
        private readonly AppDbContext _context;
        private readonly IExpenseService _expenseService;

        private Dictionary<int,string> monthsVal = new Dictionary<int, string>
        {
            {1, "Jan"},
            {2, "Feb"},
            {3, "Mar"},
            {4, "Apr"},
            {5, "May"},
            {6, "Jun"},
            {7, "Jul"},
            {8, "Aug"},
            {9, "Sep"},
            {10,"Oct"},
            {11,"Nov"},
            {12,"Dec"}

        };

        public DashboardServiceImpl(AppDbContext context, IExpenseService expenseService)
        {
            _context = context;
            _expenseService = expenseService;
        }

        public async Task<Object> GetDashboardData(string username)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);

            var cardResult = await GetCardData(user.Id, username);
            var monthlySavingsResult = await GetMonthlySavings(user.Id);
            var monthlyGoalResult = await GetMonthlyGoalData(user.Id);
            var monthlyExceedResult = await GetMonthlyExceedData(user.Id);
            var monthlyExpsneByCategoryResult = await GetMonthlyExpenseByCategory(user.Id);


            return new { cardResult, monthlySavingsResult, monthlyGoalResult, monthlyExceedResult, monthlyExpsneByCategoryResult };

        }

        public async Task<List<CardDto>> GetCardData(int userId, string username)
        {
            var cardData = new List<CardDto>();
            DateTime now = DateTime.Now;

            var details = await _context.UserDetails.SingleOrDefaultAsync(u => u.UserId == userId);

            cardData.Add(new CardDto("Monthly Income", details.SalaryPerMonth));

            var totalCapAmount = await _context.RecurringCategories
                                .Where(rc => rc.UserId == userId)
                                .SumAsync(rc => rc.CapAmount);

            var estimatedSavings = Math.Round(details.SalaryPerMonth - totalCapAmount, 2);

            cardData.Add(new CardDto("Estimated Saving", estimatedSavings));

            var totalExpense = await _context.Expenses
                    .Where(e => e.UserId == userId && e.ExpenseDate.Year == now.Year)
                    .SumAsync(e => e.Amount);

            cardData.Add(new CardDto("Total Spent", Math.Round(totalExpense, 2)));

            if (!await _expenseService.CheckComplete(username)) {

                var totalCap = await _context.RecurringCategories
                             .Where(rc => rc.UserId == userId)
                             .SumAsync(rc => rc.CapAmount);

                

                var difference = details.SalaryPerMonth - totalExpense;

                cardData.Add(new CardDto("Current Savings", Math.Round(details.CurrentBalance + difference, 2)));
            }
            else
                cardData.Add(new CardDto("Current Savings", Math.Round(details.CurrentBalance, 2)));

            return cardData;


        }

        public async Task<List<MonthlySavingsDto>> GetMonthlySavings(int userId)
        {
            DateTime now = DateTime.Now;

            var result = await _context.MarkCompletes
                        .Where(u => u.UserId == userId && u.Year == now.Year)
                        .Select(u => new MonthlySavingsDto { Month = monthsVal[u.Month], Savings = u.Savings }).ToListAsync();

            if (result.Count > 0) {
                return result;
            }

            return new List<MonthlySavingsDto>();


        }

        public async Task<MonthlyGoalDto> GetMonthlyGoalData(int userId)
        {
            DateTime now = DateTime.Now;

            var details = await _context.UserDetails.SingleOrDefaultAsync(u => u.UserId == userId);

            var totalCap = await _context.RecurringCategories
                          .Where(rc => rc.UserId == userId)
                          .SumAsync(rc => rc.CapAmount);

            var totalExpense = await _context.Expenses
                .Where(e => e.UserId == userId && e.ExpenseDate.Year == now.Year)
                .SumAsync(e => e.Amount);

            var estimatedSavings = Math.Round(details.SalaryPerMonth - totalCap, 2); 

            var currentSavings = Math.Round(details.SalaryPerMonth - totalExpense, 2);

            return new MonthlyGoalDto
            {
                Goal = estimatedSavings,
                Current = currentSavings
            };
        }

        //public async Task<List<MonthlyExceedDto>> GetMonthlyExceedData(int userId)
        //{
        //    DateTime now = DateTime.Now;

        //    var query = from ec in _context.ExpenseCategories
        //                join rc in _context.RecurringCategories on ec.Id equals rc.ExpenseCategoriesId
        //                join e in _context.Expenses on rc.ExpenseCategoriesId equals e.ExpenseCategoriesId
        //                where e.UserId == userId && e.ExpenseDate.Month == now.Month
        //                group e by new { ec.Category, rc.CapAmount } into g
        //                let spent = g.Sum(x => x.Amount)
        //                let diff = g.Key.CapAmount - spent
        //                where diff < 0
        //                orderby diff
        //                select new MonthlyExceedDto
        //                {
        //                    Category = g.Key.Category,
        //                    CapAmount = g.Key.CapAmount,
        //                    SpentAmount = spent
        //                };

        //    var result = await query.ToListAsync();





        //    return result;

        //}

        public async Task<List<MonthlyExceedDto>> GetMonthlyExceedData(int userId)
        {
            DateTime now = DateTime.Now;

            var query = from ec in _context.ExpenseCategories
                        join rc in _context.RecurringCategories on ec.Id equals rc.ExpenseCategoriesId
                        join e in _context.Expenses on rc.ExpenseCategoriesId equals e.ExpenseCategoriesId
                        where e.UserId == userId && e.ExpenseDate.Month == now.Month
                        group e by new { ec.Category, rc.CapAmount } into g
                        select new MonthlyExceedDto
                        {
                            Category = g.Key.Category,
                            CapAmount = g.Key.CapAmount,
                            SpentAmount = g.Sum(x => x.Amount)
                        };

            var result = await query
                .Where(x => x.CapAmount - x.SpentAmount < 0)
                .OrderBy(x => x.CapAmount - x.SpentAmount)
                .ToListAsync();

            return result;
        }



        public async Task<Dictionary<string, List<MonthlyExpenseDto>>> GetMonthlyExpenseByCategory(int userId)
        {
            var query = from ec in _context.ExpenseCategories
                        join rc in _context.RecurringCategories on ec.Id equals rc.ExpenseCategoriesId
                        join e in _context.Expenses on rc.ExpenseCategoriesId equals e.ExpenseCategoriesId
                        where e.UserId == userId
                        group e by new { ec.Category, rc.CapAmount, Month = e.ExpenseDate.Month } into g
                        select new
                        {
                            g.Key.Category,
                            g.Key.CapAmount,
                            g.Key.Month,
                            Spent = g.Sum(x => x.Amount)
                        };

            var result = await query.ToListAsync();

            Dictionary<string, List<MonthlyExpenseDto>> barValues = new();

            foreach (var val in result)
            {
                if (!barValues.ContainsKey(val.Category))
                    barValues[val.Category] = new List<MonthlyExpenseDto>();

                barValues[val.Category].Add(new MonthlyExpenseDto
                {
                    Month = monthsVal[val.Month],   // assuming monthsVal is a dictionary mapping month number → string
                    Amount = val.Spent,
                    Cap = val.CapAmount
                });
            }

            return barValues;
        }


    }
}
