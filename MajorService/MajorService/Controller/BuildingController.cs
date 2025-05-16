using Microsoft.AspNetCore.Mvc;
using MajorService.Business.Dtos;
using MajorService.Business.Dtos.Building;
using MajorService.Business.Services.BuildingServices;
using MajorService.Entities;
using MajorService.Middleware;
using MajorService.Utils.Filter;
using MajorService.Utils.Pagination;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MajorService.Controller
{
    [ApiController]
    [Route("api/m/[controller]")]
    public class BuildingController : ControllerBase
    {
        private readonly IBuildingSvc _buildingSvc;

        public BuildingController(IBuildingSvc buildingSvc)
        {
            _buildingSvc = buildingSvc;
        }
        [HttpGet]
        public async Task<ApiResponse<List<BuildingReadDto>>> GetAllBuildingsAsync()
        {
            var buildings = await _buildingSvc.GetAllBuildingsAsync();
            return ApiResponse<List<BuildingReadDto>>.SuccessResponse(buildings);
        }

        [HttpGet("{id}")]
        public async Task<ApiResponse<BuildingReadDto>> GetBuildingByIdAsync(Guid id)
        {
            var building = await _buildingSvc.GetBuildingByIdAsync(id);
            return ApiResponse<BuildingReadDto>.SuccessResponse(building);
        }

        [HttpPost]
        public async Task<ApiResponse<BuildingReadDto>> CreateNewBuildingAsync([FromBody] CreateNewBuildingDto request)
        {
            try
            {
                var building = await _buildingSvc.CreateNewBuildingAsync(request);
                return ApiResponse<BuildingReadDto>.SuccessResponse(building);
            }
            catch (InvalidOperationException ex)
            {
                return ApiResponse<BuildingReadDto>.ErrorResponse(new List<string> { ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return ApiResponse<BuildingReadDto>.ErrorResponse(new List<string> { ex.Message });
            }
        }

        [HttpPost("{id}/deactivate")]
        public async Task<ApiResponse<bool>> DeactivateBuildingAsync(Guid id)
        {
            var deactivateDto = new DeactivateDto { Id = id };
            var result = await _buildingSvc.DeactivateBuildingAsync(deactivateDto);
            if (result)
            {
                return ApiResponse<bool>.SuccessResponse(true);
            }
            return ApiResponse<bool>.ErrorResponse(new List<string> { "Failed to deactivate building" });
        }

        [HttpPost("{id}/activate")]
        public async Task<ApiResponse<bool>> ActivateBuildingAsync(Guid id)
        {
            var activateDto = new ActivateDto { Id = id };
            var result = await _buildingSvc.ActivateBuildingAsync(activateDto);
            if (result)
            {
                return ApiResponse<bool>.SuccessResponse(true);
            }
            return ApiResponse<bool>.ErrorResponse(new List<string> { "Failed to activate building" });
        }        [HttpGet("page")]
        public async Task<ApiResponse<BuildingListResponse>> GetByPagination([FromQuery] GetBuildingByPaginationParam param)
        {
            var result = await _buildingSvc.GetBuildingsByPaginationAsync(param.Pagination, param.Filter, param.Order);
            return ApiResponse<BuildingListResponse>.SuccessResponse(result);
        }
        
        [HttpPut("{id}")]
        public async Task<ApiResponse<BuildingReadDto>> UpdateBuildingAsync(Guid id, [FromBody] UpdateBuildingDto request)
        {
            try
            {
                var building = await _buildingSvc.UpdateBuildingAsync(id, request);
                return ApiResponse<BuildingReadDto>.SuccessResponse(building);
            }
            catch (InvalidOperationException ex)
            {
                return ApiResponse<BuildingReadDto>.ErrorResponse(new List<string> { ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return ApiResponse<BuildingReadDto>.ErrorResponse(new List<string> { ex.Message });
            }
        }
    }
}
