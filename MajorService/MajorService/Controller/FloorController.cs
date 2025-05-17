using Microsoft.AspNetCore.Mvc;
using MajorService.Business.Dtos;
using MajorService.Business.Dtos.Floor;
using MajorService.Business.Services.FloorServices;
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
    public class FloorController : ControllerBase
    {
        private readonly IFloorSvc _floorSvc;

        public FloorController(IFloorSvc floorSvc)
        {
            _floorSvc = floorSvc;
        }

        [HttpGet]
        public async Task<ApiResponse<List<FloorReadDto>>> GetAllFloorsAsync()
        {
            var floors = await _floorSvc.GetAllFloorsAsync();
            return ApiResponse<List<FloorReadDto>>.SuccessResponse(floors);
        }

        [HttpGet("all")]
        public async Task<ApiResponse<List<FloorReadDto>>> GetAllFloorsWithoutPaginationAsync()
        {
            var floors = await _floorSvc.GetAllFloorsAsync();
            return ApiResponse<List<FloorReadDto>>.SuccessResponse(floors);
        }

        [HttpGet("{id}")]
        public async Task<ApiResponse<FloorReadDto>> GetFloorByIdAsync(Guid id)
        {
            var floor = await _floorSvc.GetFloorByIdAsync(id);
            return ApiResponse<FloorReadDto>.SuccessResponse(floor);
        }

        [HttpPost]
        public async Task<ApiResponse<FloorReadDto>> CreateNewFloorAsync([FromBody] CreateNewFloorDto request)
        {
            try
            {
                var floor = await _floorSvc.CreateNewFloorAsync(request);
                return ApiResponse<FloorReadDto>.SuccessResponse(floor);
            }
            catch (InvalidOperationException ex)
            {
                return ApiResponse<FloorReadDto>.ErrorResponse(new List<string> { ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return ApiResponse<FloorReadDto>.ErrorResponse(new List<string> { ex.Message });
            }
        }

        [HttpPost("{id}/deactivate")]
        public async Task<ApiResponse<bool>> DeactivateFloorAsync(Guid id)
        {
            var deactivateDto = new DeactivateDto { Id = id };
            var result = await _floorSvc.DeactivateFloorAsync(deactivateDto);
            if (result)
            {
                return ApiResponse<bool>.SuccessResponse(true);
            }
            return ApiResponse<bool>.ErrorResponse(new List<string> { "Failed to deactivate floor" });
        }

        [HttpPost("{id}/activate")]
        public async Task<ApiResponse<bool>> ActivateFloorAsync(Guid id)
        {
            var activateDto = new ActivateDto { Id = id };
            var result = await _floorSvc.ActivateFloorAsync(activateDto);
            if (result)
            {
                return ApiResponse<bool>.SuccessResponse(true);
            }
            return ApiResponse<bool>.ErrorResponse(new List<string> { "Failed to activate floor" });
        }        
        [HttpGet("page")]
        public async Task<ApiResponse<FloorListResponse>> GetByPagination([FromQuery] GetFloorByPaginationParam param)
        {
            var result = await _floorSvc.GetFloorsByPaginationAsync(param.Pagination, param.Filter, param.Order);
            return ApiResponse<FloorListResponse>.SuccessResponse(result);
        }
        
        [HttpPut("{id}")]
        public async Task<ApiResponse<FloorReadDto>> UpdateFloorAsync(Guid id, [FromBody] UpdateFloorDto request)
        {
            try
            {
                var floor = await _floorSvc.UpdateFloorAsync(id, request);
                return ApiResponse<FloorReadDto>.SuccessResponse(floor);
            }
            catch (KeyNotFoundException ex)
            {
                return ApiResponse<FloorReadDto>.ErrorResponse(new List<string> { ex.Message });
            }
        }

        [HttpGet("byLocation/{locationId}")]
        public async Task<ApiResponse<List<FloorReadDto>>> GetFloorsByLocationIdAsync(Guid locationId)
        {
            var floors = await _floorSvc.GetFloorsByLocationIdAsync(locationId);
            return ApiResponse<List<FloorReadDto>>.SuccessResponse(floors);
        }
    }
}
