using Microsoft.AspNetCore.Mvc;
using UserService.Business.Dtos.Student;

namespace UserService.Business.Services.StudentService
{
    public interface IStudentService
    {
        Task<IActionResult> CreateStudentByExcelAsync(CreateStudentByExcelDto createStudentByExcelDto);
        //Task<StudentDto> UpdateStudentAsync(Guid id, UpdateStudentDto updateStudentDto);
        Task<bool> DeleteStudentAsync(Guid id);  
        Task<List<StudentDto>> GetAllStudentsAsync();
        Task<StudentDto> GetStudentByIdAsync(Guid id);
    }
}
