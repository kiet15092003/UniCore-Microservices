using Microsoft.AspNetCore.Mvc;
using StudentService.Business.Dtos.Student;
using StudentService.Business.Services;
using StudentService.Middleware;

namespace StudentService.Controller
{
    [Route("api/s/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentSvc _studentSvc;
        public StudentController(IStudentSvc studentSvc)
        {
            _studentSvc = studentSvc;
        }

        [HttpGet]
        public async Task<ApiResponse<List<StudentReadDto>>> GetAllStudents()
        {
            var students = await _studentSvc.GetAllStudents();
            return ApiResponse<List<StudentReadDto>>.SuccessResponse(students);
        }
    }
}
