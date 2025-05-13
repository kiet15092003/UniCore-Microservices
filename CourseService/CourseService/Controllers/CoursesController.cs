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

        [HttpGet("page")]
        public async Task<ApiResponse<CourseListResponse>> GetByPagination([FromQuery] GetCourseByPaginationParam c)
        {
            var result = await _courseService.GetProductByPagination(c.Pagination, c.Filter, c.Order);
            return ApiResponse<CourseListResponse>.SuccessResponse(result);
        }        [HttpPost]
        public async Task<ApiResponse<CourseReadDto>> CreateCourse([FromBody] CourseCreateDto courseCreateDto)
        {
            // The Code field will be auto-generated as a 6-digit number in the repository
            var result = await _courseService.CreateCourseAsync(courseCreateDto);
            return ApiResponse<CourseReadDto>.SuccessResponse(result);
        }

        [HttpPut("{id}")]
        public async Task<ApiResponse<CourseReadDto>> UpdateCourse(Guid id, [FromBody] CourseUpdateDto courseUpdateDto)
        {
            var result = await _courseService.UpdateCourseAsync(id, courseUpdateDto);
            return ApiResponse<CourseReadDto>.SuccessResponse(result);
        }

        [HttpPut("{id}/deactivate")]
        public async Task<ApiResponse<CourseReadDto>> DeactivateCourse(Guid id)
        {
            var result = await _courseService.DeactivateCourseAsync(id);
            return ApiResponse<CourseReadDto>.SuccessResponse(result);
        }
    }
}
