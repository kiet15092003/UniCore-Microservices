using MajorService.Business.Dtos;
using MajorService.Business.Dtos.Department;
using MajorService.DataAccess.Repositories.DepartmentRepo;
using MajorService.Entities;

namespace MajorService.Business.Services.DepartmentServices
{
    public class DepartmentSvc : IDepartmentSvc
    {
        private readonly IDepartmentRepo _departmentRepo;
        
        public DepartmentSvc(IDepartmentRepo departmentRepo)
        {
            _departmentRepo = departmentRepo;
        }
        
        public async Task<List<Department>> GetAllDepartmentsAsync()
        {
            return await _departmentRepo.GetAllDepartmentsAsync();
        }
        
        public async Task<Department> GetDepartmentByIdAsync(Guid id)
        {
            return await _departmentRepo.GetDepartmentByIdAsync(id);
        }
        
        public async Task<Department> CreateDepartmentAsync(DepartmentCreateDto departmentCreateDto)
        {
            var department = new Department
            {
                Name = departmentCreateDto.Name,
                Code = departmentCreateDto.Code,
                IsActive = true
            };
            
            return await _departmentRepo.CreateDepartmentAsync(department);
        }
        
        public async Task<bool> DeactivateDepartmentAsync(DeactivateDto deactivateDto)
        {
            return await _departmentRepo.DeactivateDepartmentAsync(deactivateDto.Id);
        }
    }
}