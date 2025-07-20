using MajorService.Business.Dtos;
using MajorService.Business.Dtos.Major;
using MajorService.Business.Services.MajorServices;
using MajorService.Middleware;
using MajorService.Utils.Filter;
using MajorService.Utils.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MajorService.Controller
{
    [Route("api/m/[controller]")]
    [ApiController]
    //[Authorize]
    public class MajorController : ControllerBase
    {
        private readonly IMajorSvc _majorSvc;

        public MajorController(IMajorSvc majorSvc)
        {
            _majorSvc = majorSvc;
        }

        [HttpGet]
        public async Task<ApiResponse<List<MajorReadDto>>> GetAllMajorAsync()
        {
            var majors = await _majorSvc.GetAllMajorAsync();
            return ApiResponse<List<MajorReadDto>>.SuccessResponse(majors);
        }
        [HttpPost]
        public async Task<ApiResponse<MajorReadDto>> CreateNewMajorAsync(CreateNewMajorDto request)
        {
            try
            {
                var major = await _majorSvc.CreateNewMajorAsync(request);
                return ApiResponse<MajorReadDto>.SuccessResponse(major);
            }
            catch (InvalidOperationException ex)
            {
                return ApiResponse<MajorReadDto>.ErrorResponse([ex.Message]);
            }
        }

        [HttpPut("{id}")]
        public async Task<ApiResponse<MajorReadDto>> UpdateMajorAsync(Guid id, [FromBody] UpdateMajorDto request)
        {
            try
            {
                var major = await _majorSvc.UpdateMajorAsync(id, request);
                return ApiResponse<MajorReadDto>.SuccessResponse(major);
            }
            catch (KeyNotFoundException ex)
            {
                return ApiResponse<MajorReadDto>.ErrorResponse([ex.Message]);
            }
            catch (InvalidOperationException ex)
            {
                return ApiResponse<MajorReadDto>.ErrorResponse([ex.Message]);
            }
            catch (Exception ex)
            {
                return ApiResponse<MajorReadDto>.ErrorResponse([$"An error occurred while updating the major: {ex.Message}"]);
            }
        }        
        [HttpPost("{id}/deactivate")]
        public async Task<ApiResponse<bool>> DeactivateMajorAsync(Guid id)
        {
            var deactivateDto = new DeactivateDto { Id = id };
            var result = await _majorSvc.DeactivateMajorAsync(deactivateDto);
            if (result)
            {
                return ApiResponse<bool>.SuccessResponse(true);
            }
            return ApiResponse<bool>.ErrorResponse(["Failed to deactivate major"]);
        }
        
        [HttpPost("{id}/activate")]
        public async Task<ApiResponse<bool>> ActivateMajorAsync(Guid id)
        {
            var activateDto = new ActivateDto { Id = id };
            var result = await _majorSvc.ActivateMajorAsync(activateDto);
            if (result)
            {
                return ApiResponse<bool>.SuccessResponse(true);
            }
            return ApiResponse<bool>.ErrorResponse(["Failed to activate major"]);
        }
        
        [HttpGet("page")]
        public async Task<ApiResponse<MajorListResponse>> GetByPagination([FromQuery] GetMajorByPaginationParam param)
        {
            var result = await _majorSvc.GetMajorsByPaginationAsync(param.Pagination, param.Filter, param.Order);
            return ApiResponse<MajorListResponse>.SuccessResponse(result);
        }

        [HttpDelete("{id}")]
        public async Task<ApiResponse<bool>> DeleteMajor(Guid id)
        {
            try
            {
                var result = await _majorSvc.DeleteMajorAsync(id);
                return ApiResponse<bool>.SuccessResponse(result);
            }
            catch (KeyNotFoundException ex)
            {
                return ApiResponse<bool>.ErrorResponse(new List<string> { ex.Message });
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(new List<string> { $"An error occurred while deleting the major: {ex.Message}" });
            }
        }
    }
}
