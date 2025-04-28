using CourseService.Business.Dtos.TrainingRoadmap;
using CourseService.Business.Services;
using Microsoft.AspNetCore.Mvc;
using UserService.Middleware;

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
    }
}