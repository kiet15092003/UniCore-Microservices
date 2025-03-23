using CourseService.Business.Dtos.Course;

namespace CourseService.Business.Services
{
    public interface ICourseService
    {
        Task<List<CourseReadDto>> GetCoursesAsync();
        Task<CourseReadDto> CreateCourseAsync(CourseCreateDto courseCreateDto);
    }
}
