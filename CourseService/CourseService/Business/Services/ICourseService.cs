using CourseService.Business.Dtos.Course;
using CourseService.Utils.Filter;
using CourseService.Utils.Pagination;

namespace CourseService.Business.Services
{
    public interface ICourseService
    {
        Task<CourseReadDto> CreateCourseAsync(CourseCreateDto courseCreateDto);
        Task<CourseListResponse> GetProductByPagination(Pagination pagination, CourseListFilterParams courseListFilterParams, Order? order);
        Task<CourseReadDto> UpdateCourseAsync(Guid id, CourseUpdateDto courseUpdateDto);
        Task<CourseReadDto> DeactivateCourseAsync(Guid id);
        Task<List<CourseReadDto>> GetCoursesByMajorIdAsync(Guid majorId);
    }
}
