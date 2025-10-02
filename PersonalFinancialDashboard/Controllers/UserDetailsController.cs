
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalFinancialDashboard.DTOs;
using PersonalFinancialDashboard.Entities;
using PersonalFinancialDashboard.Services.Interface;
using System.Security.Claims;

namespace PersonalFinancialDashboard.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserDetailsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        private readonly IUserDetailsService _userDetailsService;

        public UserDetailsController(IConfiguration configuration, IUserDetailsService userDetailsService)
        {
            _configuration = configuration;
            _userDetailsService = userDetailsService;
        }


        [HttpPost("profile-data")]
        public async Task<ActionResult> SetProfileData(UserDetailsDto request)
        {

            string username = User.FindFirstValue(ClaimTypes.Name);

            var result = await _userDetailsService.AddProfileData(request, username);

            if(result.Username != "")
            {
                if (result.IsAlreadyExists)
                {

                    return BadRequest("User Already exists");
                }

                else
                {
                    Response.Cookies.Delete("onboarding_required");

                    return Ok(result);
                }
            }
            return BadRequest("No user");
            
           
        }

        [HttpPut("update-profile-data")]
        public async Task<ActionResult> UpdateProfileData(UserDetailsDto request)
        {

            string username = User.FindFirstValue(ClaimTypes.Name);


            var result = await _userDetailsService.UpdateProfileData(request, username);

            if (result)
            {
                return Ok("Updated");
            }

            return BadRequest("No user");

        }

        [HttpGet("profile")]
        public async Task<ActionResult> GetProfileData()
        {
            string username = User.FindFirstValue(ClaimTypes.Name);


            var result = await _userDetailsService.GetProfileData(username);

            if (result.Username != "")
            {
                if (result.IsAlreadyExists)
                {
                    return Ok(result);
                    
                }

                else
                {
                    return BadRequest("Complete Your Profile");
                }
            }
            return BadRequest("No user");

        }


       
    }
}
