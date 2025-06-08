using CourseService.Business.Dtos.CoursesGroup;
using CourseService.Utils.Pagination;

namespace CourseService.Business.Services
{
    public interface ICoursesGroupService
    {
        Task<CoursesGroupReadDto> CreateCoursesGroupAsync(CoursesGroupCreateDto coursesGroupCreateDto);
        Task<IEnumerable<CoursesGroupReadDto>> CreateMultipleCoursesGroupsAsync(IEnumerable<CoursesGroupCreateDto> coursesGroupCreateDtos);
        Task<CoursesGroupListResponse> GetCoursesGroupsByPaginationAsync(Pagination pagination, Order? order);
        Task<CoursesGroupReadDto?> GetCoursesGroupByIdAsync(Guid id);
        Task<IEnumerable<CoursesGroupReadDto>> GetCoursesGroupsByMajorIdAsync(Guid majorId);
        Task<IEnumerable<CoursesGroupReadDto>> GetCoursesGroupsWithAllCoursesOpenForAllAsync();
    }
}