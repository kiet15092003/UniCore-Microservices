using Microsoft.AspNetCore.Mvc;
using UserService.Business.Dtos.Guardian    ;
using UserService.Business.Services.GuardianService;
using UserService.Middleware;

namespace UserService.Controllers
{
    [Route("api/u/[controller]")]
    [ApiController]
    public class GuardianController : ControllerBase {
        private readonly IGuardianService _guardianService;
        private readonly ILogger<GuardianController> _logger;

        public GuardianController(IGuardianService guardianService, ILogger<GuardianController> logger)
        {
            _guardianService = guardianService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGuardian([FromBody] CreateGuardianDto createGuardianDto)
        {
            var guardian = await _guardianService.CreateGuardianAsync(createGuardianDto);
            return Ok(guardian);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGuardian(Guid id, [FromBody] UpdateGuardianDto updateGuardianDto)
        {
            var guardian = await _guardianService.UpdateGuardianAsync(id, updateGuardianDto);
            return Ok(guardian);
        }

    }
}