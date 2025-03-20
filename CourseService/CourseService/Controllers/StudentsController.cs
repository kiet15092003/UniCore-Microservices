using CourseService.Business.Dtos.Student;
using CourseService.Business.Services.StudentService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserService.Middleware;

namespace CourseService.Controllers
{
    [Route("api/c/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;
        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        public async Task<ApiResponse<IEnumerable<StudentReadDto>>> GetAllStudents()
        {
            var students = await _studentService.GetAllStudentAsync();
            return ApiResponse<IEnumerable<StudentReadDto>>.SuccessResponse(students);
        }
    }
}
