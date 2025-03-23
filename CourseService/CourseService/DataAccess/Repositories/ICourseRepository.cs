using CourseService.Entities;

namespace CourseService.DataAccess.Repositories
{
    public interface ICourseRepository
    {
        Task<Course> CreateCourseAsync(Course course);
        Task<List<Course>> GetCoursesAsync();   
    }
}
