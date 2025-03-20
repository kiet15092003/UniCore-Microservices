using CourseService.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CourseService.DataAccess.Repositories.StudentRepo
{
    public interface IStudentRepo
    {
        Task<IEnumerable<Student>> GetAllStudentAsync();
        Task<Student> CreateStudentAsync(Student student);
    }
}
