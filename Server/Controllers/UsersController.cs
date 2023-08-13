using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseController<User>
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService) : base(userService)
        {
            _userService = userService;
        }

        [Authorize(Roles = "Freelancer")]
        [HttpGet("test")]
        public IActionResult Test()
        {
            string token = Request.Headers["Authorization"];

            if (token.StartsWith("Bearer"))
            {
                token = token.Substring("Bearer ".Length).Trim();
            }
            var handler = new JwtSecurityTokenHandler();

            JwtSecurityToken jwt = handler.ReadJwtToken(token);

            var claims = new Dictionary<string, string>();

            foreach (var claim in jwt.Claims)
            {
                claims.Add(claim.Type, claim.Value);
            }

            return Ok(claims);
        }

        [HttpPut("rate")]
        public async Task<IActionResult> RateUser(Guid userId, double ratePoint)
        {
            try
            {
                var res = await _userService.RateUser(userId, ratePoint);
                return Ok(res);

            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("suggest-freelancer")]
        public async Task<IActionResult> GetSuggestFreelancer(string workField, string fieldTags) 
        {
            try
            {
                var res = await _userService.GetSuggestFreelancer(workField, fieldTags);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
