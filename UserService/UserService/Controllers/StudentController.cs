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
            _logger.LogInformation("-----------------------------------51", JsonSerializer.Serialize(students));
            return ApiResponse<StudentListResponse>.SuccessResponse(students);
        }
    }
}

