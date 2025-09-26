using PersonalFinancialDashboard.DTOs;

namespace PersonalFinancialDashboard.Services.Interface
{
    public interface IRecurringCategoriesService
    {
        Task<RecurringCatMessageDto> AddRecurringCategoryAsync(RecurringCategoriesDto request, string username);
        Task<List<RecurringCategoriesDto>> GetSelectedCategoriesAsync(string username);
        Task<List<string>> GetRemainingCategoriesAsync(string username);
        Task<RecurringCatMessageDto> UpdateRecurringCategoryAsync(RecurringCategoriesDto request, string username);
        Task<bool> DeleteRecurringCategoryAsync(int catId, string username);
    }
}
