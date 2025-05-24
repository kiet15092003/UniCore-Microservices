using MajorService.Business.Dtos;
using MajorService.Business.Dtos.Location;
using MajorService.Business.Services.LocationServices;
using MajorService.Entities;
using MajorService.Middleware;
using MajorService.Utils.Filter;
using MajorService.Utils.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace MajorService.Controller
{
    [Route("api/m/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationSvc _locationSvc;

        public LocationController(ILocationSvc locationSvc)
        {
            _locationSvc = locationSvc;
        }
        [HttpGet]
        public async Task<ApiResponse<List<LocationReadDto>>> GetAllLocationsAsync()
        {
            var locations = await _locationSvc.GetAllLocationsAsync();
            return ApiResponse<List<LocationReadDto>>.SuccessResponse(locations);
        }

        [HttpGet("{id}")]
        public async Task<ApiResponse<LocationReadDto>> GetLocationByIdAsync(Guid id)
        {
            var location = await _locationSvc.GetLocationByIdAsync(id);
            return ApiResponse<LocationReadDto>.SuccessResponse(location);
        }

        [HttpPost]
        public async Task<ApiResponse<LocationReadDto>> CreateNewLocationAsync([FromBody] CreateNewLocationDto request)
        {
            try
            {
                var location = await _locationSvc.CreateNewLocationAsync(request);
                return ApiResponse<LocationReadDto>.SuccessResponse(location);
            }
            catch (InvalidOperationException ex)
            {
                return ApiResponse<LocationReadDto>.ErrorResponse(new List<string> { ex.Message });
            }
        }

        [HttpPost("{id}/deactivate")]
        public async Task<ApiResponse<bool>> DeactivateLocationAsync(Guid id)
        {
            var deactivateDto = new DeactivateDto { Id = id };
            var result = await _locationSvc.DeactivateLocationAsync(deactivateDto);
            if (result)
            {
                return ApiResponse<bool>.SuccessResponse(true);
            }
            return ApiResponse<bool>.ErrorResponse(new List<string> { "Failed to deactivate location" });
        }

        [HttpPost("{id}/activate")]
        public async Task<ApiResponse<bool>> ActivateLocationAsync(Guid id)
        {
            var activateDto = new ActivateDto { Id = id };
            var result = await _locationSvc.ActivateLocationAsync(activateDto);
            if (result)
            {
                return ApiResponse<bool>.SuccessResponse(true);
            }
            return ApiResponse<bool>.ErrorResponse(new List<string> { "Failed to activate location" });
        }
        [HttpGet("page")]
        public async Task<ApiResponse<LocationListResponse>> GetByPagination([FromQuery] GetLocationByPaginationParam param)
        {
            var result = await _locationSvc.GetLocationsByPaginationAsync(param.Pagination, param.Filter, param.Order);
            return ApiResponse<LocationListResponse>.SuccessResponse(result);
        }

        [HttpPut("{id}")]
        public async Task<ApiResponse<LocationReadDto>> UpdateLocationAsync(Guid id, [FromBody] UpdateLocationDto request)
        {
            try
            {
                var location = await _locationSvc.UpdateLocationAsync(id, request);
                return ApiResponse<LocationReadDto>.SuccessResponse(location);
            }
            catch (InvalidOperationException ex)
            {
                return ApiResponse<LocationReadDto>.ErrorResponse(new List<string> { ex.Message });
            }
            catch (ArgumentException ex)
            {
                return ApiResponse<LocationReadDto>.ErrorResponse(new List<string> { ex.Message });
            }
        }
    }
}
