using UserService.Entities;
using UserService.Business.Dtos.Student;
using UserService.Utils.Pagination;
using UserService.Utils.Filter;

namespace UserService.DataAccess.Repositories.StudentRepo
{
    public interface IStudentRepo
    {
        Task<Student> GetStudentByIdAsync(Guid id);
        Task<PaginationResult<StudentDto>> GetAllPaginationAsync(Pagination pagination, StudentListFilterParams filter, Order order);
        Task<Student> CreateAsync(Student student);
        Task<Student> UpdateAsync(Student student); Task<bool> DeleteAsync(Guid id); Task SaveChangesAsync();
        Task<(List<ApplicationUser> Users, List<Student> Students)> AddStudentsWithUsersAsync(
            List<(ApplicationUser User, Student Student)> userStudentPairs,
            Dictionary<string, string>? passwords = null);
        Task<string> UpdateStudentImageAsync(Guid id, string imageUrl);
        Task<Student> GetStudentDetailByIdAsync(Guid id);
        Task<Student> GetStudentByEmailAsync(string email);
    }
}