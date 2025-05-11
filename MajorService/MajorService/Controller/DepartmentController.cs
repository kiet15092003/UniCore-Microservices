using MajorService.Business.Dtos;
using MajorService.Business.Dtos.Department;
using MajorService.Business.Services.DepartmentServices;
using MajorService.Entities;
using MajorService.Middleware;
using Microsoft.AspNetCore.Mvc;

namespace MajorService.Controller
{
    [Route("api/m/[controller]")]
    [ApiController]
    //[Authorize]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentSvc _departmentSvc;

        public DepartmentController(IDepartmentSvc departmentSvc)
        {
            _departmentSvc = departmentSvc;
        }

        [HttpGet]
        public async Task<ApiResponse<List<Department>>> GetAllDepartmentsAsync()
        {
            var departments = await _departmentSvc.GetAllDepartmentsAsync();
            return ApiResponse<List<Department>>.SuccessResponse(departments);
        }
        
        [HttpGet("{id}")]
        public async Task<ApiResponse<Department>> GetDepartmentByIdAsync(Guid id)
        {
            var department = await _departmentSvc.GetDepartmentByIdAsync(id);
            return ApiResponse<Department>.SuccessResponse(department);
        }
        
        [HttpPost]
        public async Task<ApiResponse<Department>> CreateDepartmentAsync(DepartmentCreateDto departmentCreateDto)
        {
            var department = await _departmentSvc.CreateDepartmentAsync(departmentCreateDto);
            return ApiResponse<Department>.SuccessResponse(department);
        }
        
        [HttpPost("deactivate")]
        public async Task<ApiResponse<bool>> DeactivateDepartmentAsync(DeactivateDto deactivateDto)
        {
            var result = await _departmentSvc.DeactivateDepartmentAsync(deactivateDto);
            if (result)
            {
                return ApiResponse<bool>.SuccessResponse(true);
            }
            return ApiResponse<bool>.ErrorResponse(["Failed to deactivate department"]);
        }
    }
}