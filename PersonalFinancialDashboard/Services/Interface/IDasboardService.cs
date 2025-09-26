using PersonalFinancialDashboard.DTOs;

namespace PersonalFinancialDashboard.Services.Interface
{
    public interface IDasboardService
    {
        Task<List<CardDto>> GetCardData(int userId, string username);
        Task<List<MonthlySavingsDto>> GetMonthlySavings(int userId);
        Task<MonthlyGoalDto> GetMonthlyGoalData(int userId);
        Task<List<MonthlyExceedDto>> GetMonthlyExceedData(int userId);

        Task<Dictionary<string, List<MonthlyExpenseDto>>> GetMonthlyExpenseByCategory(int userId);
        Task<Object> GetDashboardData(string username);
    }
}
