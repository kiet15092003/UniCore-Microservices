using Microsoft.AspNetCore.Mvc;
using UserService.Business.Services.AuthService;
using UserService.Middleware;
using UserService.Business.Dtos.Auth;
using Microsoft.AspNetCore.Authorization;

namespace UserService.Controllers
{
    [Route("api/u/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]

    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _config;
        public AuthController(
            IAuthService authService,
            IConfiguration config)
        {
            _authService = authService;
            _config = config;
        }

        [HttpPost("students/register")]
        public async Task<ApiResponse<IActionResult>> RegisterStudent([FromBody] RegisterStudentDto model)
        {
            var result = await _authService.RegisterStudentAsync(model);
            return ApiResponse<IActionResult>.SuccessResponse(result);
        }

        [HttpPost("trainingManagers/register")]
        public async Task<ApiResponse<IActionResult>> RegisterTrainingManager([FromBody] RegisterTrainingManagerDto model)
        {
            var result = await _authService.RegisterTrainingManagerAsync(model);
            return ApiResponse<IActionResult>.SuccessResponse(result);
        }

        [HttpPost("login")]
        public async Task<ApiResponse<string>> Login([FromBody] LoginDto model)
        {
            var result = await _authService.LoginAsync(model);
            return ApiResponse<string>.SuccessResponse(result);
        }

        //[HttpGet("validate")]
        //public IActionResult ValidateToken()
        //{
        //    var authHeader = Request.Headers["Authorization"].FirstOrDefault();
        //    if (authHeader == null || !authHeader.StartsWith("Bearer "))
        //    {
        //        return Unauthorized(new { message = "Missing or invalid token" });
        //    }

        //    var token = authHeader.Substring("Bearer ".Length);
        //    var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);

        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    try
        //    {
        //        var claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
        //        {
        //            ValidateIssuerSigningKey = true,
        //            IssuerSigningKey = new SymmetricSecurityKey(key),
        //            ValidateIssuer = true,
        //            ValidateAudience = true,
        //            ValidateLifetime = true,
        //            ValidIssuer = _config["Jwt:Issuer"],
        //            ValidAudience = _config["Jwt:Audience"],
        //            ClockSkew = TimeSpan.Zero
        //        }, out var validatedToken);

        //        var jwtToken = (JwtSecurityToken)validatedToken;
        //        var roles = jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();

        //        return Ok(new { isValid = true, roles });
        //    }
        //    catch (SecurityTokenExpiredException)
        //    {
        //        return Unauthorized(new { message = "Token has expired" });
        //    }
        //    catch (SecurityTokenInvalidSignatureException)
        //    {
        //        return Unauthorized(new { message = "Invalid token signature" });
        //    }
        //    catch
        //    {
        //        return Unauthorized(new { message = "Invalid token" });
        //    }
        //}
    }
}
