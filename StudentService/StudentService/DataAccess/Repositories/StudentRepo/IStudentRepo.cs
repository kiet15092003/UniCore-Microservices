using StudentService.Entities;

namespace StudentService.DataAccess.Repositories.StudentRepo
{
    public interface IStudentRepo
    {
        Task<Student> CreateStudentAsync(Student student);
        Task<List<Student>> GetAllStudentsAsync();

        Task AddRangeAsync(IEnumerable<Student> students);
        Task<Student?> GetStudentByIdAsync(Guid id);
        Task UpdateStudentAsync(Student student);
        Task DeleteStudentAsync(Student student);
        Task SaveChangesAsync();
    }
}
