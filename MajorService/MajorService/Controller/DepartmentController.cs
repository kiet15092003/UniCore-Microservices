using MajorService.Business.Dtos;
using MajorService.Business.Dtos.Department;
using MajorService.Business.Services.DepartmentServices;
using MajorService.Entities;
using MajorService.Middleware;
using MajorService.Utils.Filter;
using MajorService.Utils.Pagination;
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
        public async Task<ApiResponse<Department>> CreateNewDepartmentAsync([FromBody] CreateNewDepartmentDto request)
        {
            try
            {
                var department = await _departmentSvc.CreateNewDepartmentAsync(request.Name);
                return ApiResponse<Department>.SuccessResponse(department);
            }
            catch (InvalidOperationException ex)
            {
                return ApiResponse<Department>.ErrorResponse([ex.Message]);
            }
        }          
        [HttpPost("{id}/deactivate")]
        public async Task<ApiResponse<bool>> DeactivateDepartmentAsync(Guid id)
        {
            var deactivateDto = new DeactivateDto { Id = id };
            var result = await _departmentSvc.DeactivateDepartmentAsync(deactivateDto);
            if (result)
            {
                return ApiResponse<bool>.SuccessResponse(true);
            }
            return ApiResponse<bool>.ErrorResponse(["Failed to deactivate department"]);
        }
          
        [HttpPost("{id}/activate")]
        public async Task<ApiResponse<bool>> ActivateDepartmentAsync(Guid id)
        {
            var activateDto = new ActivateDto { Id = id };
            var result = await _departmentSvc.ActivateDepartmentAsync(activateDto);
            if (result)
            {
                return ApiResponse<bool>.SuccessResponse(true);
            }
            return ApiResponse<bool>.ErrorResponse(["Failed to activate department"]);
        }
        
        [HttpGet("page")]
        public async Task<ApiResponse<DepartmentListResponse>> GetByPagination([FromQuery] GetDepartmentByPaginationParam param)
        {
            var result = await _departmentSvc.GetDepartmentsByPaginationAsync(
                param.Pagination, param.Filter, param.Order);
            return ApiResponse<DepartmentListResponse>.SuccessResponse(result);
        }
    }
}