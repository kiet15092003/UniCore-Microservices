using Microsoft.AspNetCore.Mvc;
using UserService.Middleware;
using UserService.Business.Dtos.Lecturer;
using Microsoft.AspNetCore.Authorization;
using UserService.Business.Services.LecturerService;
using UserService.Utils.Pagination;
using UserService.Utils.Filter;
using System.Text.Json;

namespace UserService.Controllers
{
    [Route("api/u/[controller]")]
    [ApiController]
    public class LecturerController : ControllerBase
    {
        private readonly ILecturerService _lecturerService;
        private readonly ILogger<LecturerController> _logger;

        public LecturerController(ILecturerService lecturerService, ILogger<LecturerController> logger)
        {
            _lecturerService = lecturerService;
            _logger = logger;
        }


        [HttpGet("all")]
        public async Task<ApiResponse<LecturerListResponse>> GetAllLecturers(
            [FromQuery] Pagination pagination,
            [FromQuery] LecturerListFilterParams filter,
            [FromQuery] Order? order)
        {
            var lecturers = await _lecturerService.GetAllLecturersAsync(pagination, filter, order);
            return ApiResponse<LecturerListResponse>.SuccessResponse(lecturers);
        }

        [HttpDelete("{id}")]
        public async Task<ApiResponse<bool>> DeleteLecturer(Guid id)
        {
            try
            {
                var result = await _lecturerService.DeleteLecturerAsync(id);
                if (result)
                {
                    return ApiResponse<bool>.SuccessResponse(true);
                }
                return ApiResponse<bool>.ErrorResponse(["Lecturer not found"]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting lecturer with id {Id}", id);
                return ApiResponse<bool>.ErrorResponse([$"Error deleting lecturer: {ex.Message}"]);
            }
        }

        [HttpPut("{id}")]
        public async Task<ApiResponse<LecturerDto>> UpdateLecturer(Guid id, UpdateLecturerDto updateLecturerDto)
        {
            try
            {
                var result = await _lecturerService.UpdateLecturerAsync(id, updateLecturerDto);
                if (result != null)
                {
                    return ApiResponse<LecturerDto>.SuccessResponse(result);
                }
                return ApiResponse<LecturerDto>.ErrorResponse(["Lecturer not found"]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating lecturer with id {Id}", id);
                return ApiResponse<LecturerDto>.ErrorResponse([$"Error updating lecturer: {ex.Message}"]);
            }
        }

        [HttpGet("{id}")]
        public async Task<ApiResponse<LecturerDetailDto>> GetLecturerById(Guid id)
        {
            try
            {
                var result = await _lecturerService.GetLecturerDetailByIdAsync(id);
                if (result != null)
                {
                    return ApiResponse<LecturerDetailDto>.SuccessResponse(result);
                }
                return ApiResponse<LecturerDetailDto>.ErrorResponse(["Lecturer not found"]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting lecturer with id {Id}", id);
                return ApiResponse<LecturerDetailDto>.ErrorResponse([$"Error getting lecturer: {ex.Message}"]);
            }
        }

        [HttpPut("update-image")]
        public async Task<ApiResponse<string>> UpdateUserImage(UpdateUserImageDto updateUserImageDto)
        {
            var result = await _lecturerService.UpdateUserImageAsync(updateUserImageDto);
            return ApiResponse<string>.SuccessResponse(result);
        }

        [HttpGet("get-lecturer-by-email")]
        public async Task<ApiResponse<LecturerDto>> GetLecturerByEmail(string email)
        {
            var result = await _lecturerService.GetLecturerByEmailAsync(email);
            return ApiResponse<LecturerDto>.SuccessResponse(result);
        }

        [HttpPost]
        public async Task<ApiResponse<LecturerDto>> CreateLecturer(CreateLecturerDto createLecturerDto)
        {
            try
            {
                var result = await _lecturerService.CreateLecturerAsync(createLecturerDto);
                if (result is OkObjectResult okResult)
                {
                    return ApiResponse<LecturerDto>.SuccessResponse((LecturerDto)okResult.Value);
                }
                else if (result is BadRequestObjectResult badRequestResult)
                {
                    string errorMessage = badRequestResult.Value?.ToString() ?? "Failed to create lecturer";
                    return ApiResponse<LecturerDto>.ErrorResponse([errorMessage]);
                }
                return ApiResponse<LecturerDto>.ErrorResponse(["Failed to create lecturer"]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating lecturer");
                return ApiResponse<LecturerDto>.ErrorResponse([$"Error creating lecturer: {ex.Message}"]);
            }
        }

        [HttpPost("by-majors-department")]
        public async Task<ApiResponse<List<LecturerDto>>> GetLecturersByMajorsDepartment([FromBody] List<string> majorIds)
        {
            var result = await _lecturerService.GetLecturersByMajorsDepartmentAsync(majorIds);
            return ApiResponse<List<LecturerDto>>.SuccessResponse(result);
        }
    }
} 