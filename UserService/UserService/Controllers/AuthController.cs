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

        [HttpPost("login")]
        public async Task<ApiResponse<string>> Login([FromBody] LoginDto model)
        {
            var result = await _authService.LoginAsync(model);
            return ApiResponse<string>.SuccessResponse(result);
        }
    }
}
