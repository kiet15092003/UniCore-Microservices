using CourseService.Entities;
using CourseService.Utils.Filter;
using CourseService.Utils.Pagination;

namespace CourseService.DataAccess.Repositories
{
    public interface ICourseRepository
    {
        Task<Course> CreateCourseAsync(Course course);
        Task<PaginationResult<Course>> GetAllCoursesPaginationAsync(
                Pagination pagination,
                CourseListFilterParams courseListFilterParams,
                Order? order);
        Task<Course> GetCourseByIdAsync(Guid id);
        Task<Course> UpdateCourseAsync(Course course);
    }
}
