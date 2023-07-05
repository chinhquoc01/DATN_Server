using Application.Services;
using AutoMapper;
using Domain.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Collections;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public AuthController(IConfiguration config, IUserService userService, IMapper mapper)
        {
            _config = config;
            _userService = userService;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginParam loginParam)
        {
            var user = await _userService.GetUserLogin(loginParam);
            if (user == null)
            {
                return NoContent();
            }
            var token = GenerateJSONWebToken(user);
            object res = new
            {
                token = token,
                userInfo = user,
                userType = user.UserType
            };
            return Ok(res);
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(SignupParam signupParam)
        {
            try
            {
                var res = await _userService.SignUp(signupParam);
                return Ok(res);

            }
            catch (ValidateException ex)
            {
                return HandleValidateException(ex);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        private string GenerateJSONWebToken(UserDTO userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.Name),
                new Claim(JwtRegisteredClaimNames.Email, userInfo.Email),
                new Claim("Phone", userInfo.Phone),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, userInfo.UserType.ToString())
            };
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private IActionResult HandleException(Exception ex)
        {
            var error = new ErrorResponse
            {
                DevMsg = ex.Message,
                UserMsg = Domain.Resources.ResourceVN.Error_Exception
            };
            return StatusCode(500, error);
        }

        private IActionResult HandleValidateException(ValidateException ex)
        {
            // Lấy ra lỗi đầu tiên gặp phải rồi gán cho userMsg
            var userMsg = new List<string>();
            var e = ex.Data?.GetEnumerator();
            if(e != null)
            {
                e.MoveNext();
                DictionaryEntry userMsgPair = (DictionaryEntry)e.Current;

                userMsg = (List<string>)userMsgPair.Value;
            } else
            {
                userMsg.Add("Có lỗi xảy ra");
            }

            var error = new ErrorResponse
            {
                DevMsg = ex.Message,
                Data = ex.Data,
                UserMsg = userMsg?.FirstOrDefault(),
            };
            return BadRequest(error);
        }
    }
}
