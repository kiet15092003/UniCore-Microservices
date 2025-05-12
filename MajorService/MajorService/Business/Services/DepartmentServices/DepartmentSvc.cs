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
    }
}