using AutoMapper;
using MajorService.Business.Dtos;
using MajorService.Business.Dtos.Department;
using MajorService.DataAccess.Repositories.DepartmentRepo;
using MajorService.Entities;
using MajorService.Utils.Filter;
using MajorService.Utils.Pagination;

namespace MajorService.Business.Services.DepartmentServices
{
    public class DepartmentSvc : IDepartmentSvc
    {
        private readonly IDepartmentRepo _departmentRepo;
        private readonly IMapper _mapper;
        
        public DepartmentSvc(IDepartmentRepo departmentRepo, IMapper mapper)
        {
            _departmentRepo = departmentRepo;
            _mapper = mapper;
        }
        
        public async Task<List<Department>> GetAllDepartmentsAsync()
        {
            return await _departmentRepo.GetAllDepartmentsAsync();
        }
        
        public async Task<Department> GetDepartmentByIdAsync(Guid id)
        {
            return await _departmentRepo.GetDepartmentByIdAsync(id);
        }        public async Task<Department> CreateNewDepartmentAsync(string departmentName)
        {
            // Check if department name already exists
            bool nameExists = await _departmentRepo.IsDepartmentNameExistsAsync(departmentName);
            if (nameExists)
            {
                throw new InvalidOperationException($"Department with name '{departmentName}' already exists.");
            }
            
            // Generate a unique code with the format UNIDEPART-[6 digits]
            string uniqueCode = await _departmentRepo.GenerateUniqueCodeAsync();
            
            // Create the department
            var department = new Department
            {
                Name = departmentName,
                Code = uniqueCode,
                IsActive = true
            };
            
            return await _departmentRepo.CreateDepartmentAsync(department);
        }
          public async Task<bool> DeactivateDepartmentAsync(DeactivateDto deactivateDto)
        {
            return await _departmentRepo.DeactivateDepartmentAsync(deactivateDto.Id);
        }
        
        public async Task<bool> ActivateDepartmentAsync(ActivateDto activateDto)
        {
            return await _departmentRepo.ActivateDepartmentAsync(activateDto.Id);
        }
        
        public async Task<DepartmentListResponse> GetDepartmentsByPaginationAsync(
            Pagination pagination, 
            DepartmentListFilterParams departmentListFilterParams, 
            Order? order)
        {
            var result = await _departmentRepo.GetDepartmentsByPaginationAsync(
                pagination, 
                departmentListFilterParams, 
                order);
            
            var dtos = _mapper.Map<List<DepartmentReadDto>>(result.Data);
            
            var response = new DepartmentListResponse
            {
                Data = dtos,
                Total = result.Total,
                PageSize = result.PageSize,
                PageIndex = result.PageIndex
            };
            
            return response;
        }

        public async Task<bool> DeleteDepartmentAsync(Guid id)
        {
            // Check if department exists
            try
            {
                var department = await _departmentRepo.GetDepartmentByIdAsync(id);
                
                // Check if department has any major groups
                var majorGroups = await _departmentRepo.GetMajorGroupsByDepartmentIdAsync(id);
                if (majorGroups.Any())
                {
                    throw new InvalidOperationException($"Cannot delete department '{department.Name}' because it has {majorGroups.Count} associated major group(s). Please remove all major groups before deleting the department.");
                }

                // If no major groups, proceed with deletion
                var result = await _departmentRepo.DeleteDepartmentAsync(id);
                return result;
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException("Department not found");
            }
        }
    }
}