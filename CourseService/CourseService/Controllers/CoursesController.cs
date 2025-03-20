using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CourseService.Controllers
{
    [Route("api/c/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        [Authorize(Roles = "TrainingManager")]
        [HttpGet]
        public async Task<IActionResult> GetCourses()
        {
            var user = HttpContext.User;
            var roles = user.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            return Ok(new { message = "Viewing all courses", roles });
        }
    }
}
