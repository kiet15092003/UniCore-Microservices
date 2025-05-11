using UserService.Entities;

namespace UserService.DataAccess.Repositories.StudentRepo
{
    public interface IStudentRepo
    {
        Task<Student> GetStudentByIdAsync(Guid id);
        Task<List<Student>> GetAllAsync();
        Task<Student> CreateAsync(Student student);
        Task<Student> UpdateAsync(Student student);
        Task<bool> DeleteAsync(Guid id);
        Task SaveChangesAsync();
        Task<(List<ApplicationUser> Users, List<Student> Students)> AddStudentsWithUsersAsync(List<(ApplicationUser User, Student Student)> userStudentPairs);
    }
}