using CourseService.Business.Dtos.Student;
using CourseService.Entities;

namespace CourseService.Business.Services.StudentService
{
    public interface IStudentService
    {
        Task<IEnumerable<StudentReadDto>> GetAllStudentAsync();
    }
}
