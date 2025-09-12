using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PersonalFinancialDashboard.Entities;
using PersonalFinancialDashboard.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PersonalFinancialDashboard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public AuthController(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDtos request)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            {
                return BadRequest("Username already exists");
            }

            var user = new User { Username = request.Username };
            var passwordHasher = new PasswordHasher<User>();
            user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDtos request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null) return BadRequest(new { error = "User does not exist" });

            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (result == PasswordVerificationResult.Failed) return BadRequest(new { error = "Wrong Password" });

            string token = CreateToken(user); // Your token creation logic here

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,   // Or None if HTTPS and cross-origin local dev
                Expires = DateTime.UtcNow.AddHours(1),
                Path = "/"
            };

            Response.Cookies.Append("jwt_token", token, cookieOptions);

            return Ok(new { message = "Logged in successfully" });
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassowrd([FromBody] ChangePasswordDto password)
        {
            string username = User.FindFirstValue(ClaimTypes.Name);
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (user == null) return BadRequest(new { error = "User does not exist" });

            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password.CurrentPassword);
            if (result == PasswordVerificationResult.Failed) return BadRequest(new { error = "Wrong Password" });

            string token = CreateToken(user); // Your token creation logic here

           
        
            user.PasswordHash = passwordHasher.HashPassword(user, password.NewPassword);

           
            await _context.SaveChangesAsync();

            return Ok(new { message = "Password Changed successfully" });
        }


        private string CreateToken(User user)
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Username),
        // add other claims if needed
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AppSettings:Token"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["AppSettings:Issuer"],
                audience: _configuration["AppSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
