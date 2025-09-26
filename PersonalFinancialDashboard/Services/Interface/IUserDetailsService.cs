using PersonalFinancialDashboard.DTOs;
using PersonalFinancialDashboard.Entities;

namespace PersonalFinancialDashboard.Services.Interface
{
    public interface IUserDetailsService
    {
        Task<ProfileDto> AddProfileData(UserDetailsDto request, string username);
        Task<bool> UpdateProfileData(UserDetailsDto request, string username);
        Task<ProfileDto> GetProfileData(string username);
    }
}
