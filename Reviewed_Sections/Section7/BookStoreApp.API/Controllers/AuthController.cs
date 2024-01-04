using AutoMapper;
using BookStoreApp.API.Data;
using BookStoreApp.API.Models.User;
using BookStoreApp.API.Static;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookStoreApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;
        private readonly UserManager<ApiUser> _userManager;

        public AuthController(ILogger<AuthController> logger, IMapper mapper, UserManager<ApiUser> userManager, IConfiguration configuration)
        {
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(UserDto userDto)
        {
            //if(userDto == null) { return BadRequest("Insufficient Data Provided"); }// BadRequest(ModelState)            

            _logger.LogInformation($"Registration Attempt for {userDto.Email}");

            try
            {
                var user = _mapper.Map<ApiUser>(userDto);
                // Without this line (below) we get error InvalidUserName:
                // "UserName '' is invalid, can only contain letters or digits"
                user.UserName = userDto.Email;

                var result = await _userManager.CreateAsync(user, userDto.Password);

                if (result.Succeeded == false)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }

                    return BadRequest(ModelState);
                }

                // Here we also can add User Claims if needed

                await _userManager.AddToRoleAsync(user, userDto.Role);

                return Accepted();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(Register)}");

                return Problem($"Something Went Wrong in the {nameof(Register)}", statusCode: 500);
            }            
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginUserDto userDto)
        {
            _logger.LogInformation($"Login Attempt for {userDto.Email}");

            try
            {
                var user = await _userManager.FindByEmailAsync(userDto.Email);
                // For Web App we need signInManager, cookies retain state,
                // api not so why used CheckPassword
                var passwordValid = await _userManager.CheckPasswordAsync(user, userDto.Password);

                if (user == null || passwordValid == false)
                {
                    return Unauthorized(userDto); // NotFound();
                }

                string tokenString = await GenerateToken(user);

                var response = new AuthResponse
                {
                    UserId = user.Id,
                    Token = tokenString,
                    Email = userDto.Email
                };

                return response; // Accepted()
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(Login)}");

                return Problem($"Something Went Wrong in the {nameof(Login)}", statusCode: 500);
            }
        }

        private async Task<string> GenerateToken(ApiUser user)
        {
            var securityKey = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]);
            var issuerSigningKey = new SymmetricSecurityKey(securityKey);
            var credentials = new SigningCredentials(issuerSigningKey, SecurityAlgorithms.HmacSha256);

            var roles = await _userManager.GetRolesAsync(user);

            var roleClaims = roles.Select(r => new Claim(ClaimTypes.Role, r)).ToList();

            // In case if we during the registration gave claims to user
            // we can retrieve it back into the JWT payload
            var userClaims = await _userManager.GetClaimsAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName), // Sub is subject and subject is User
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Jti against Replay attacks
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(CustomClaimTypes.Uid, user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(Convert.ToInt32(_configuration["JwtSettings:Duration"])),
                signingCredentials: credentials
            );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return jwtToken;

        }
    }
}
