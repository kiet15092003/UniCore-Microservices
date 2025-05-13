using Microsoft.AspNetCore.Mvc;
using UserService.Business.Dtos.Student;
using UserService.Utils.Pagination;
using UserService.Utils.Filter;

namespace UserService.Business.Services.StudentService
{
    public interface IStudentService
    {
        Task<IActionResult> CreateStudentByExcelAsync(CreateStudentByExcelDto createStudentByExcelDto);
        //Task<StudentDto> UpdateStudentAsync(Guid id, UpdateStudentDto updateStudentDto);
        Task<bool> DeleteStudentAsync(Guid id);  
        Task<StudentListResponse> GetAllStudentsAsync(Pagination pagination, StudentListFilterParams filter, Order? order);
        Task<StudentDto> GetStudentByIdAsync(Guid id);
        // Task<PaginationResult<StudentDto>> GetAllPaginationAsync(Pagination pagination, StudentListFilterParams filter, Order order);
    }
}
