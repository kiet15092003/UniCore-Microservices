using CourseService.Business.Dtos.Course;
using CourseService.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserService.Middleware;

namespace CourseService.Controllers
{
    [Route("api/c/[controller]")]
    [ApiController]
    //[Authorize(Roles = "TrainingManager")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;
        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        public async Task<ApiResponse<List<CourseReadDto>>> GetCourses()
        {
            var result = await _courseService.GetCoursesAsync();
            return ApiResponse<List<CourseReadDto>>.SuccessResponse(result);
        }
        [HttpPost]
        public async Task<ApiResponse<CourseReadDto>> CreateCourse([FromBody] CourseCreateDto courseCreateDto)
        {
            var result = await _courseService.CreateCourseAsync(courseCreateDto);
            return ApiResponse<CourseReadDto>.SuccessResponse(result);
        }
    }
}
