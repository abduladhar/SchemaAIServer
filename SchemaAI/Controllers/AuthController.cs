using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SchemaAI.Entities;
using SchemaAI.ServiceContract;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SchemaAI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config, IUserService userService)
        {
            _config = config;
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
                return BadRequest("Username and password are required.");

            try
            {
                var user = await _userService.LoginAsync(model);
                if (user == null)
                    return Unauthorized("Invalid credentials");

                // Generate JWT or Base64 token
                var token = GenerateJwtToken(user);

                var response = new
                {
                    user = new
                    {
                        Use = user.UserGuid,
                        username = user.UserName,
                        name = user.UserName,
                        phone = user.Phone,
                        email = user.Email
                        

                    },
                    token = token
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] LoginModel model)
        {
            try
            {
                await _userService.ResetPassword(model);

                return Ok("New Password Send to Email");
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        private string GenerateJwtToken(User p_objUser)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("UserGuid", p_objUser.UserGuid.ToString()),
                new Claim("Name", p_objUser.UserName),
                new Claim("Phone", p_objUser.Phone),
                new Claim("TenantGuid", p_objUser.TenantGuid.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(Convert.ToDouble(jwtSettings["ExpireDay"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
