
using Microsoft.EntityFrameworkCore;
using PersonalFinancialDashboard.DTOs;
using PersonalFinancialDashboard.Entities;
using PersonalFinancialDashboard.Services.Interface;

namespace PersonalFinancialDashboard.Services
{
    public class UserDetailsServiceImpl : IUserDetailsService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public UserDetailsServiceImpl(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<ProfileDto> AddProfileData(UserDetailsDto request, string username)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);

            if (user != null)
            {
                var details = await _context.UserDetails.FirstOrDefaultAsync(u => u.UserId == user.Id);
                if (details != null)
                    return (new ProfileDto ( details.SalaryPerMonth, details.CurrentBalance, details.Id, details.UserId, username, true ));
                var userDetails = new UserDetails
                {
                    CurrentBalance = request.CurrentBalance,
                    SalaryPerMonth = request.SalaryPerMonth,
                    UserId = user.Id, // Set FK directly

                };

                _context.UserDetails.Add(userDetails);
                await _context.SaveChangesAsync();

                return (new ProfileDto( userDetails.SalaryPerMonth, userDetails.CurrentBalance, userDetails.Id, userDetails.UserId, username, false ));
            }

            return (new ProfileDto ( 0.0, 0.0, 0, 0, "", false ));
        }

        public async Task<bool> UpdateProfileData(UserDetailsDto request, string username)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);

            if (user != null)
            {
                var userDetails = await _context.UserDetails.SingleOrDefaultAsync(u => u.UserId == user.Id);

                userDetails.SalaryPerMonth = request.SalaryPerMonth;
                userDetails.CurrentBalance = request.CurrentBalance;


                await _context.SaveChangesAsync();

                return true;
            }

            return false;
        }
        public async Task<ProfileDto> GetProfileData(string username)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);

            if (!string.IsNullOrEmpty(username))
            {
                var details = await _context.UserDetails.SingleOrDefaultAsync(u => u.UserId == user.Id);
                if (details != null)
                {

                    return (new ProfileDto( details.SalaryPerMonth, details.CurrentBalance, details.Id, details.UserId, username, true ));
                }
                return new ProfileDto(0.0, 0.0, 0, 0, username, false);

            }

            return new ProfileDto(0.0, 0.0, 0, 0, "", false);
        }

    }
}
