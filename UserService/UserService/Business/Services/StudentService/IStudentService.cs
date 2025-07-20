using Microsoft.AspNetCore.Mvc;
using UserService.Business.Dtos.Student;
using UserService.Utils.Pagination;
using UserService.Utils.Filter;
using UserService.Entities;

namespace UserService.Business.Services.StudentService
{
    public interface IStudentService
    {
        Task<IActionResult> CreateStudentByExcelAsync(CreateStudentByExcelDto createStudentByExcelDto);
        Task<IActionResult> CreateStudentAsync(CreateStudentDto createStudentDto);
        Task<StudentDto> UpdateStudentAsync(Guid id, UpdateStudentDto updateStudentDto);
        Task<bool> DeleteStudentAsync(Guid id);  
        Task<StudentListResponse> GetAllStudentsAsync(Pagination pagination, StudentListFilterParams filter, Order? order);
        Task<StudentDto> GetStudentByIdAsync(Guid id);
        Task<StudentDetailDto> GetStudentDetailByIdAsync(Guid id);
        Task<string> UpdateUserImageAsync(UpdateUserImageDto updateUserImageDto);
        Task<StudentDto> GetStudentByEmailAsync(string email);
        // Task<PaginationResult<StudentDto>> GetAllPaginationAsync(Pagination pagination, StudentListFilterParams filter, Order order);
    }
}
