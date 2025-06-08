using CourseService.Business.Dtos.TrainingRoadmap;
using CourseService.Business.Services;
using Microsoft.AspNetCore.Mvc;
using CourseService.Middleware;

namespace CourseService.Controllers
{
    [Route("api/c/[controller]")]
    [ApiController]
    //[Authorize(Roles = "TrainingManager")]
    public class TrainingRoadmapsController : ControllerBase
    {
        private readonly ITrainingRoadmapService _trainingRoadmapService;
        
        public TrainingRoadmapsController(ITrainingRoadmapService trainingRoadmapService)
        {
            _trainingRoadmapService = trainingRoadmapService;
        }

        [HttpGet("{id}")]
        public async Task<  ApiResponse<TrainingRoadmapReadDto>> GetTrainingRoadmapById(Guid id)
        {
            var result = await _trainingRoadmapService.GetTrainingRoadmapByIdAsync(id);
            if (result == null)
            {
                return ApiResponse<TrainingRoadmapReadDto>.ErrorResponse(new List<string> { "Training roadmap not found" });
            }
            return ApiResponse<TrainingRoadmapReadDto>.SuccessResponse(result);
        }

        [HttpGet("page")]
        public async Task<ApiResponse<TrainingRoadmapListResponse>> GetByPagination([FromQuery] GetTrainingRoadmapByPaginationParam param)
        {
            var result = await _trainingRoadmapService.GetTrainingRoadmapsByPagination(
                param.Pagination, param.Filter, param.Order);
            return ApiResponse<TrainingRoadmapListResponse>.SuccessResponse(result);
        }

        [HttpPost]
        public async Task<ApiResponse<TrainingRoadmapReadDto>> CreateTrainingRoadmap([FromBody] TrainingRoadmapCreateDto createDto)
        {
            var result = await _trainingRoadmapService.CreateTrainingRoadmapAsync(createDto);
            return ApiResponse<TrainingRoadmapReadDto>.SuccessResponse(result);
        }

        [HttpPut("{id}")]
        public async Task<ApiResponse<TrainingRoadmapReadDto>> UpdateTrainingRoadmap(
            Guid id, [FromBody] TrainingRoadmapUpdateDto updateDto)
        {
            var result = await _trainingRoadmapService.UpdateTrainingRoadmapAsync(id, updateDto);
            return ApiResponse<TrainingRoadmapReadDto>.SuccessResponse(result);
        }

        [HttpPost("components")]
        public async Task<ApiResponse<TrainingRoadmapReadDto>> UpdateTrainingRoadmapComponents([FromBody] TrainingRoadmapAddComponentsDto componentsDto)
        {
            try
            {
                var result = await _trainingRoadmapService.AddTrainingRoadmapComponentsAsync(componentsDto);
                return ApiResponse<TrainingRoadmapReadDto>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                return ApiResponse<TrainingRoadmapReadDto>.ErrorResponse(new List<string> { ex.Message });
            }
        }

        [HttpPost("{id}/deactivate")]
        public async Task<ApiResponse<TrainingRoadmapReadDto>> DeactivateTrainingRoadmap(Guid id)
        {
            var result = await _trainingRoadmapService.DeactivateTrainingRoadmapAsync(id);
            if (result == null)
            {
                return ApiResponse<TrainingRoadmapReadDto>.ErrorResponse(new List<string> { "Training roadmap not found" });
            }
            return ApiResponse<TrainingRoadmapReadDto>.SuccessResponse(result);
        }

        [HttpPost("{id}/activate")]
        public async Task<ApiResponse<TrainingRoadmapReadDto>> ActivateTrainingRoadmap(Guid id)
        {
            var result = await _trainingRoadmapService.ActivateTrainingRoadmapAsync(id);
            if (result == null)
            {
                return ApiResponse<TrainingRoadmapReadDto>.ErrorResponse(new List<string> { "Training roadmap not found" });
            }
            return ApiResponse<TrainingRoadmapReadDto>.SuccessResponse(result);
        }
    }
}