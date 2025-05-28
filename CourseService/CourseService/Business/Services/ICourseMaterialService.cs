using CourseService.Business.Dtos.Course;
using CourseService.Business.Dtos.Material;
using CourseService.Middleware;
using CourseService.Utils.Filter;
using CourseService.Utils.Pagination;

namespace CourseService.Business.Services
{
    public interface ICourseMaterialService
    {
        Task<ApiResponse<IEnumerable<CourseMaterialReadDto>>> GetMaterialsByCourseIdAsync(Guid courseId);
        Task<ApiResponse<CourseMaterialReadDto>> GetMaterialByIdAsync(Guid courseId, Guid materialId);
        Task<ApiResponse<CourseMaterialReadDto>> AddMaterialAsync(CourseMaterialCreateDto createDto, Guid courseId);
        Task<ApiResponse<CourseMaterialReadDto>> UpdateMaterialAsync(Guid courseId, CourseMaterialUpdateDto updateDto);
        Task<ApiResponse<bool>> DeleteMaterialAsync(Guid courseId, Guid materialId);
        
        // Phương thức phân trang
        Task<ApiResponse<PaginationResult<MaterialReadDto>>> GetMaterialsPaginationAsync(
            Guid courseId,
            Pagination pagination,
            MaterialListFilterParams filterParams,
            Order? order);
    }
} 