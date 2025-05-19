using Microsoft.AspNetCore.Mvc;
using UserService.Middleware;
using UserService.Business.Dtos.Student;
using Microsoft.AspNetCore.Authorization;
using UserService.Business.Services.StudentService;
using UserService.Utils.Pagination;
using UserService.Utils.Filter;
using System.Text.Json;
namespace UserService.Controllers
{
    [Route("api/u/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly ILogger<StudentController> _logger;

        public StudentController(IStudentService studentService, ILogger<StudentController> logger)
        {
            _studentService = studentService;
            _logger = logger;

        }

        [HttpPost("register/excel")]
        public async Task<ApiResponse<object>> RegisterStudentsFromExcel(CreateStudentByExcelDto model)
        {
            if (model.ExcelFile == null || model.ExcelFile.Length == 0)
            {
                return ApiResponse<object>.ErrorResponse(["File is empty or not provided."]);
            }

            try
            {
                var result = await _studentService.CreateStudentByExcelAsync(model);
                if (result is OkObjectResult okResult)
                {
                    return ApiResponse<object>.SuccessResponse(okResult.Value);
                }
                else if (result is BadRequestObjectResult badRequest)
                {
                    return ApiResponse<object>.ErrorResponse([badRequest.Value.ToString()]);
                }
                return ApiResponse<object>.ErrorResponse(["Unknown error occurred"]);
            }
            catch (Exception ex)
            {
                return ApiResponse<object>.ErrorResponse([$"Error processing file: {ex.Message}"]);
            }
        }

        [HttpGet("all")]
        public async Task<ApiResponse<StudentListResponse>> GetAllStudents(
            [FromQuery] Pagination pagination,
            [FromQuery] StudentListFilterParams filter,
            [FromQuery] Order? order)
        {
            var students = await _studentService.GetAllStudentsAsync(pagination, filter, order);
            return ApiResponse<StudentListResponse>.SuccessResponse(students);
        }

        [HttpDelete("{id}")]
        public async Task<ApiResponse<bool>> DeleteStudent(Guid id)
        {
            try
            {
                var result = await _studentService.DeleteStudentAsync(id);
                if (result)
                {
                    return ApiResponse<bool>.SuccessResponse(true);
                }
                return ApiResponse<bool>.ErrorResponse(["Student not found"]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting student with id {Id}", id);
                return ApiResponse<bool>.ErrorResponse([$"Error deleting student: {ex.Message}"]);
            }
        }

        [HttpPut("{id}")]
        public async Task<ApiResponse<StudentDto>> UpdateStudent(Guid id, UpdateStudentDto updateStudentDto)
        {
            try
            {
                var result = await _studentService.UpdateStudentAsync(id, updateStudentDto);
                if (result != null)
                {
                    return ApiResponse<StudentDto>.SuccessResponse(result);
                }
                return ApiResponse<StudentDto>.ErrorResponse(["Student not found"]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating student with id {Id}", id);
                return ApiResponse<StudentDto>.ErrorResponse([$"Error updating student: {ex.Message}"]);
            }
        }
    }
}

