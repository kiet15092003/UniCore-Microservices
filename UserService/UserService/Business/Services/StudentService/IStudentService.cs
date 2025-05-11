using Microsoft.AspNetCore.Mvc;
using UserService.Business.Dtos.Student;

namespace UserService.Business.Services.StudentService
{
    public interface IStudentService
    {
        Task<IActionResult> CreateStudentByExcelAsync(CreateStudentByExcelDto createStudentByExcelDto);
    }
}
