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

        [HttpPost("upload")]
        public async Task<ApiResponse<string>> AddStudentsFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return ApiResponse<string>.ErrorResponse(["File is empty or not provided."]);
            }

            try
            {
                var students = await _studentSvc.AddStudentsFromExcel(file);
                return ApiResponse<string>.SuccessResponse("Students added successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.ErrorResponse([$"Error processing file: {ex.Message}"]);
            }
        }

        [HttpPost]
        public async Task<ApiResponse<StudentReadDto>> CreateStudent([FromBody] StudentCreateDto studentDto)
        {
            try
            {
                var student = await _studentSvc.CreateStudent(studentDto);
                return ApiResponse<StudentReadDto>.SuccessResponse(student);
            }
            catch (Exception ex)
            {
                return ApiResponse<StudentReadDto>.ErrorResponse([$"Error creating student: {ex.Message}"]);
            }
        }

        [HttpPut("{id}")]
        public async Task<ApiResponse<StudentReadDto>> UpdateStudent(Guid id, [FromBody] StudentUpdateDto studentDto)
        {
            try
            {
                var student = await _studentSvc.UpdateStudent(id, studentDto);
                return ApiResponse<StudentReadDto>.SuccessResponse(student);
            }
            catch (KeyNotFoundException ex)
            {
                return ApiResponse<StudentReadDto>.ErrorResponse([ex.Message]);
            }
            catch (Exception ex)
            {
                return ApiResponse<StudentReadDto>.ErrorResponse([$"Error updating student: {ex.Message}"]);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ApiResponse<bool>> DeleteStudent(Guid id)
        {
            try
            {
                var result = await _studentSvc.DeleteStudent(id);
                return ApiResponse<bool>.SuccessResponse(result);
            }
            catch (KeyNotFoundException ex)
            {
                return ApiResponse<bool>.ErrorResponse([ex.Message]);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse([$"Error deleting student: {ex.Message}"]);
            }
        }

        [HttpPost("bulk")]
        public async Task<ApiResponse<List<StudentReadDto>>> BulkCreateStudents([FromBody] BulkCreateStudentRequest bulkCreateStudentRequest)
        {
            var students = await _studentSvc.BulkCreateStudents(bulkCreateStudentRequest);
            return ApiResponse<List<StudentReadDto>>.SuccessResponse(students);
        }
    }
}
