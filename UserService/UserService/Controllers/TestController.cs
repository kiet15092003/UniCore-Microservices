using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserService.Business.Dtos.Auth;
using UserService.Business.Services.AuthService;
using UserService.CommunicationTypes.Grpc.GrpcClient;
using UserService.Middleware;

namespace UserService.Controllers
{
    [Route("api/u/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly GrpcMajorClientService _grpcClient;

        public TestController(GrpcMajorClientService grpcClient)
        {
            _grpcClient = grpcClient;
        }

        [Authorize(Roles = "TrainingManager")]
        [HttpGet("users")]
        public IActionResult GetAllUsers()
        {
            var roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
            return Ok(new { Roles = roles });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMajor(Guid id)
        {
            var response = await _grpcClient.GetMajorByIdAsync(id.ToString());

            if (!response.Success)
            {
                return BadRequest(new { response.Success, response.Error });
            }

            return Ok(new
            {
                response.Success,
                response.Data.Id,
                response.Data.Name,
                response.Data.Code
            });
        }
    }
}
