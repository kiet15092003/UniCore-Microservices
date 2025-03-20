using StudentService.Entities;

namespace StudentService.DataAccess.Repositories.StudentRepo
{
    public interface IStudentRepo
    {
        Task<Student> CreateStudentAsync(Student student);
        Task<List<Student>> GetAllStudentsAsync();  
    }
}
