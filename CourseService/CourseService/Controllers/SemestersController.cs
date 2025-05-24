using CourseService.Business.Dtos.Semester;
using CourseService.Business.Services;
using CourseService.Utils.Filter;
using CourseService.Utils.Pagination;
using Microsoft.AspNetCore.Mvc;
using UserService.Middleware;

namespace CourseService.Controllers
{
    [Route("api/c/[controller]")]
    [ApiController]
    //[Authorize(Roles = "TrainingManager")]
    public class SemestersController : ControllerBase
    {
        private readonly ISemesterService _semesterService;
        
        public SemestersController(ISemesterService semesterService)
        {
            _semesterService = semesterService;
        }

        [HttpGet("page")]
        public async Task<ApiResponse<PaginationResult<SemesterReadDto>>> GetByPagination([FromQuery] GetSemesterByPaginationParam param)
        {
            var result = await _semesterService.GetByPaginationAsync(param.Pagination, param.Filter, param.Order);
            return ApiResponse<PaginationResult<SemesterReadDto>>.SuccessResponse(result);
        }
        
        [HttpGet("{id}")]
        public async Task<ApiResponse<SemesterReadDto>> GetSemesterById(Guid id)
        {
            var result = await _semesterService.GetSemesterByIdAsync(id);
            if (result == null)
            {
                return ApiResponse<SemesterReadDto>.ErrorResponse(new List<string> { "Semester not found" });
            }
            return ApiResponse<SemesterReadDto>.SuccessResponse(result);
        }

        [HttpPost]
        public async Task<ApiResponse<SemesterReadDto>> CreateSemester([FromBody] SemesterCreateDto semesterCreateDto)
        {
            var result = await _semesterService.CreateSemesterAsync(semesterCreateDto);
            return ApiResponse<SemesterReadDto>.SuccessResponse(result);
        }

        [HttpPut("{id}")]
        public async Task<ApiResponse<SemesterReadDto>> UpdateSemester(Guid id, [FromBody] SemesterUpdateDto semesterUpdateDto)
        {
            var result = await _semesterService.UpdateSemesterAsync(id, semesterUpdateDto);
            return ApiResponse<SemesterReadDto>.SuccessResponse(result);
        }
      
        [HttpPost("{id}/deactivate")]
        public async Task<ApiResponse<SemesterReadDto>> DeactivateSemester(Guid id)
        {
            var result = await _semesterService.DeactivateSemesterAsync(id);
            return ApiResponse<SemesterReadDto>.SuccessResponse(result);
        }
    }

    public class GetSemesterByPaginationParam
    {
        public Pagination Pagination { get; set; } = new Pagination();
        public SemesterFilterParams Filter { get; set; } = new SemesterFilterParams();
        public Order? Order { get; set; }
    }
}
