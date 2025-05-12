using UserService.Entities;
using UserService.Business.Dtos.Student;
namespace UserService.DataAccess.Repositories.StudentRepo
{
    public interface IStudentRepo
    {
        Task<Student> GetStudentByIdAsync(Guid id);
        Task<List<StudentDto>> GetAllAsync();
        Task<Student> CreateAsync(Student student);
        Task<Student> UpdateAsync(Student student);
        Task<bool> DeleteAsync(Guid id);
        Task SaveChangesAsync();
        Task<(List<ApplicationUser> Users, List<Student> Students)> AddStudentsWithUsersAsync(List<(ApplicationUser User, Student Student)> userStudentPairs);
    }
}