using CourseService.Entities;

namespace CourseService.DataAccess.Repositories
{
    public interface ICourseMaterialRepository
    {
        Task<CourseMaterial> GetByIdAsync(Guid courseId, Guid materialId);
        Task<IEnumerable<CourseMaterial>> GetByCourseIdAsync(Guid courseId);
        Task<CourseMaterial> AddAsync(CourseMaterial courseMaterial);
        Task<bool> UpdateAsync(CourseMaterial courseMaterial);
        Task<bool> DeleteAsync(Guid courseId, Guid materialId);
        Task<Material> GetMaterialByIdAsync(Guid materialId);
        Task<Material> AddMaterialAsync(Material material);
        Task<bool> UpdateMaterialAsync(Material material);
        Task<bool> DeleteMaterialAsync(Guid materialId);
    }
} 