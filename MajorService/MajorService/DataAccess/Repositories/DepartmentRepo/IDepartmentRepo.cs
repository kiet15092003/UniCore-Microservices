using MajorService.Entities;
using MajorService.Utils.Filter;
using MajorService.Utils.Pagination;

namespace MajorService.DataAccess.Repositories.DepartmentRepo
{
    public interface IDepartmentRepo
    {
        Task<Department> GetDepartmentByIdAsync(Guid id);
        Task<List<Department>> GetAllDepartmentsAsync();
        Task<Department> CreateDepartmentAsync(Department department);
        Task<Department> UpdateDepartmentAsync(Department department);
        Task<bool> DeactivateDepartmentAsync(Guid id);
        Task<bool> ActivateDepartmentAsync(Guid id);
        Task<PaginationResult<Department>> GetDepartmentsByPaginationAsync(
            Pagination pagination,
            DepartmentListFilterParams departmentListFilterParams,
            Order? order);
        Task<bool> IsDepartmentNameExistsAsync(string name);
        Task<bool> IsDepartmentNameExistsForOtherAsync(Guid id, string name);
        Task<string> GenerateUniqueCodeAsync();
        Task<bool> DeleteDepartmentAsync(Guid id);
        Task<List<MajorGroup>> GetMajorGroupsByDepartmentIdAsync(Guid departmentId);
    }
}