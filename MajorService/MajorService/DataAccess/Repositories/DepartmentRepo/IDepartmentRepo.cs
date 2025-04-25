using MajorService.Entities;

namespace MajorService.DataAccess.Repositories.DepartmentRepo
{
    public interface IDepartmentRepo
    {
        Task<Department> GetDepartmentByIdAsync(Guid id);
        Task<List<Department>> GetAllDepartmentsAsync();
        Task<Department> CreateDepartmentAsync(Department department);
        Task<bool> DeactivateDepartmentAsync(Guid id);
    }
}