using PersonalFinancialDashboard.DTOs;

namespace PersonalFinancialDashboard.Services.Interface
{
    public interface IAuthService
    {
        Task<string> RegisterUser(UserDtos request);
        Task<LoginDto> Login(UserDtos request);
        Task<string> ChangePassword(ChangePasswordDto request, string username);
    
    }
}
