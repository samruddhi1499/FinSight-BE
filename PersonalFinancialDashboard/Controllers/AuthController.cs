
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinancialDashboard.DTOs;
using PersonalFinancialDashboard.Services.Interface;
using System.Security.Claims;


namespace PersonalFinancialDashboard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService;

        public AuthController(IConfiguration configuration, IAuthService authService)
        {
            _configuration = configuration;
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(UserDtos request)
        {
            string result = await _authService.RegisterUser(request);
            if (result == "Created")
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDtos request)
        {
            LoginDto result = await _authService.Login(request);

            if(result.Message == "Login Success")
            {
                Response.Cookies.Append("jwt_token", result.Token, result.CookieOptions);
                if (!result.IsOnboarded)
                {
                    Response.Cookies.Append("onboarding_required", "true", result.CookieOptions);
                }
                else
                {
                    Response.Cookies.Delete("onboarding_required");
                }

                return Ok(new { result.Message, result.IsOnboarded });
            }

            return BadRequest(new { result.Message, result.IsOnboarded });
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassowrd([FromBody] ChangePasswordDto request)
        {
            string username = User.FindFirstValue(ClaimTypes.Name);
            string result = await _authService.ChangePassword(request, username);
            if(result == "Success")
            {
                return Ok(result);
            }
            return BadRequest(result);

        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {


            Response.Cookies.Append(
                "jwt_token",
               "", // Set an empty value
               new CookieOptions()
               {
                   Expires = DateTimeOffset.UtcNow.AddDays(-1), // Set an expired date in the past
                   HttpOnly = true, // Important: Maintain HttpOnly flag if it was set on the original cookie
                   Secure = true, // Set to true if the original cookie was secure (HTTPS)
                   SameSite = SameSiteMode.None, // Adjust SameSite mode as needed, matching the original cookie
                   Path = "/",
                   Domain = ".vercel.app" // Important: Match the path of the original cookie
                              // You may also need to set the Domain property if the original cookie had one

               }
           );

            return Ok("");
         }



        
        

    }
}
