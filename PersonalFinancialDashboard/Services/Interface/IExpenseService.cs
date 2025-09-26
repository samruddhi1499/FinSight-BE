using PersonalFinancialDashboard.DTOs;

namespace PersonalFinancialDashboard.Services.Interface
{
    public interface IExpenseService
    {
        Task<string> AddExpenseAsync(ExpenseDto request, string username);
        Task<List<ExpenseSummaryDto>> GetExpensesAsync(string username, bool toBeFiltered);
        Task<List<ExpenseDetailDto>> GetExpenseByCategoryAsync(int categoryId, string? username);
        Task<bool> UpdateExpenseAmountAsync(int expenseId, float amount);
        Task<bool> DeleteExpenseAsync(int expenseId);

        Task<bool> MarkComplete(string username);

        Task<bool> CheckComplete(string username);
    }
}
