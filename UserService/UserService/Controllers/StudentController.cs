using Microsoft.AspNetCore.Mvc;
using UserService.Middleware;
using UserService.Business.Dtos.Student;
using Microsoft.AspNetCore.Authorization;
using UserService.Business.Services.StudentService;

namespace UserService.Controllers
{
    [Route("api/u/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
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
    }
}

