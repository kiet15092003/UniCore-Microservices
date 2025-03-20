using StudentService.Business.Dtos.Student;

namespace StudentService.Business.Services
{
    public interface IStudentSvc
    {
        Task<List<StudentReadDto>> GetAllStudents();
    }
}
