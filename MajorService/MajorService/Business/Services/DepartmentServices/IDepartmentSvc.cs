using MajorService.Business.Dtos;
using MajorService.Business.Dtos.Department;
using MajorService.Entities;

namespace MajorService.Business.Services.DepartmentServices
{
    public interface IDepartmentSvc
    {
        Task<List<Department>> GetAllDepartmentsAsync();
        Task<Department> GetDepartmentByIdAsync(Guid id);
        Task<Department> CreateDepartmentAsync(DepartmentCreateDto departmentCreateDto);
        Task<bool> DeactivateDepartmentAsync(DeactivateDto deactivateDto);
    }
}