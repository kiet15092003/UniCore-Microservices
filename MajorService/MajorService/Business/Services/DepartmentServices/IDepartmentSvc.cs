using MajorService.Business.Dtos;
using MajorService.Business.Dtos.Department;
using MajorService.Entities;
using MajorService.Utils.Filter;
using MajorService.Utils.Pagination;

namespace MajorService.Business.Services.DepartmentServices
{    public interface IDepartmentSvc
    {        Task<List<Department>> GetAllDepartmentsAsync();
        Task<Department> GetDepartmentByIdAsync(Guid id);
        Task<Department> CreateNewDepartmentAsync(string departmentName);
        Task<Department> UpdateDepartmentAsync(Guid id, UpdateDepartmentDto dto);
        Task<bool> DeactivateDepartmentAsync(DeactivateDto deactivateDto);
        Task<bool> ActivateDepartmentAsync(ActivateDto activateDto);
        Task<DepartmentListResponse> GetDepartmentsByPaginationAsync(
            Pagination pagination, 
            DepartmentListFilterParams departmentListFilterParams, 
            Order? order);
        Task<bool> DeleteDepartmentAsync(Guid id);
    }
}