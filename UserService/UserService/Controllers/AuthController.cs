using Microsoft.AspNetCore.Mvc;
using UserService.Business.Services.AuthService;
using UserService.Middleware;
using UserService.Business.Dtos.Auth;
using Microsoft.AspNetCore.Authorization;

namespace UserService.Controllers
{
    [Route("api/u/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(
            IAuthService authService
            )
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ApiResponse<string>> Login([FromBody] LoginDto model)
        {
            var result = await _authService.LoginAsync(model);
            return ApiResponse<string>.SuccessResponse(result);
        }
    }
}
