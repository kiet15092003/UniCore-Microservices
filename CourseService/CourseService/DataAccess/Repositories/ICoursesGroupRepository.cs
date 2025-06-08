using CourseService.Entities;
using CourseService.Utils.Filter;
using CourseService.Utils.Pagination;

namespace CourseService.DataAccess.Repositories
{
    public interface ICoursesGroupRepository
    {
        Task<CoursesGroup> CreateCoursesGroupAsync(CoursesGroup coursesGroup);
        Task<IEnumerable<CoursesGroup>> CreateMultipleCoursesGroupsAsync(IEnumerable<CoursesGroup> coursesGroups);
        Task<CoursesGroup> GetCoursesGroupByIdAsync(Guid id);
        Task<bool> GroupNameExistsInMajorAsync(string groupName, Guid? majorId, Guid? excludeId = null);
        Task<PaginationResult<CoursesGroup>> GetAllCoursesGroupsPaginationAsync(
            Pagination pagination, 
            Order? order);
        Task<CoursesGroup> UpdateCoursesGroupAsync(CoursesGroup coursesGroup);
        Task<IEnumerable<CoursesGroup>> GetCoursesGroupsByMajorIdAsync(Guid majorId);
        Task<IEnumerable<CoursesGroup>> GetCoursesGroupsWithAllCoursesOpenForAllAsync();
    }
}