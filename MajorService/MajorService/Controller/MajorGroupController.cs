using MajorService.Business.Dtos;
using MajorService.Business.Dtos.MajorGroup;
using MajorService.Business.Services.MajorGroupServices;
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
    public class MajorGroupController : ControllerBase
    {
        private readonly IMajorGroupSvc _majorGroupSvc;

        public MajorGroupController(IMajorGroupSvc majorGroupSvc)
        {
            _majorGroupSvc = majorGroupSvc;
        }

        [HttpGet]
        public async Task<ApiResponse<List<MajorGroup>>> GetAllMajorGroupsAsync()
        {
            var majorGroups = await _majorGroupSvc.GetAllMajorGroupsAsync();
            return ApiResponse<List<MajorGroup>>.SuccessResponse(majorGroups);
        }
        
        [HttpGet("{id}")]
        public async Task<ApiResponse<MajorGroup>> GetMajorGroupByIdAsync(Guid id)
        {
            var majorGroup = await _majorGroupSvc.GetMajorGroupByIdAsync(id);
            return ApiResponse<MajorGroup>.SuccessResponse(majorGroup);
        }
          [HttpPost]
        public async Task<ApiResponse<MajorGroup>> CreateNewMajorGroupAsync(CreateNewMajorGroupDto request)
        {
            try
            {
                var majorGroup = await _majorGroupSvc.CreateNewMajorGroupAsync(request);
                return ApiResponse<MajorGroup>.SuccessResponse(majorGroup);
            }
            catch (InvalidOperationException ex)
            {
                return ApiResponse<MajorGroup>.ErrorResponse([ex.Message]);
            }
        }
        
        [HttpPost("deactivate")]
        public async Task<ApiResponse<bool>> DeactivateMajorGroupAsync(DeactivateDto deactivateDto)
        {
            var result = await _majorGroupSvc.DeactivateMajorGroupAsync(deactivateDto);
            if (result)
            {
                return ApiResponse<bool>.SuccessResponse(true);
            }
            return ApiResponse<bool>.ErrorResponse(["Failed to deactivate major group"]);
        }
        
        [HttpGet("page")]
        public async Task<ApiResponse<MajorGroupListResponse>> GetByPagination([FromQuery] GetMajorGroupByPaginationParam param)
        {
            var result = await _majorGroupSvc.GetMajorGroupsByPaginationAsync(
                param.Pagination, param.Filter, param.Order);
            return ApiResponse<MajorGroupListResponse>.SuccessResponse(result);
        }
    }
}