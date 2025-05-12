using MajorService.Business.Dtos;
using MajorService.Business.Dtos.Department;
using MajorService.Entities;
using MajorService.Utils.Filter;
using MajorService.Utils.Pagination;

namespace MajorService.Business.Services.DepartmentServices
{
    public interface IDepartmentSvc
    {
        Task<List<Department>> GetAllDepartmentsAsync();
        Task<Department> GetDepartmentByIdAsync(Guid id);
        Task<Department> CreateDepartmentAsync(DepartmentCreateDto departmentCreateDto);
        Task<bool> DeactivateDepartmentAsync(DeactivateDto deactivateDto);
        Task<DepartmentListResponse> GetDepartmentsByPaginationAsync(
            Pagination pagination, 
            DepartmentListFilterParams departmentListFilterParams, 
            Order? order);
    }
}