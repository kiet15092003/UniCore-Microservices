using CourseService.Business.Dtos.AcademicClass;
using CourseService.Business.Services;
using Microsoft.AspNetCore.Mvc;
using CourseService.Middleware;

namespace CourseService.Controllers
{
    [Route("api/c/[controller]")]
    [ApiController]
    public class AcademicClassesController : ControllerBase
    {
        private readonly IAcademicClassService _academicClassService;

        public AcademicClassesController(IAcademicClassService academicClassService)
        {
            _academicClassService = academicClassService;
        }

        [HttpGet("page")]
        public async Task<ApiResponse<AcademicClassListResponse>> GetAcademicClassesByPagination([FromQuery] GetAcademicClassByPaginationParam param)
        {
            var result = await _academicClassService.GetAllAcademicClassesPaginationAsync(param.Pagination, param.Filter, param.Order);
            return ApiResponse<AcademicClassListResponse>.SuccessResponse(result);
        }

        [HttpGet("{id}")]
        public async Task<ApiResponse<AcademicClassReadDto>> GetAcademicClassById(Guid id)
        {
            var academicClass = await _academicClassService.GetAcademicClassByIdAsync(id);
            return ApiResponse<AcademicClassReadDto>.SuccessResponse(academicClass);
        }

        [HttpGet("course/{courseId}")]
        public async Task<ApiResponse<List<AcademicClassReadDto>>> GetAcademicClassesByCourseId(Guid courseId)
        {
            var academicClasses = await _academicClassService.GetAcademicClassesByCourseIdAsync(courseId);

            return ApiResponse<List<AcademicClassReadDto>>.SuccessResponse(academicClasses);
        }

        [HttpGet("semester/{semesterId}")]
        public async Task<ApiResponse<List<AcademicClassReadDto>>> GetAcademicClassesBySemesterId(Guid semesterId)
        {
            var academicClasses = await _academicClassService.GetAcademicClassesBySemesterIdAsync(semesterId);

            return ApiResponse<List<AcademicClassReadDto>>.SuccessResponse(academicClasses);
        }

        [HttpPost]
        public async Task<ApiResponse<AcademicClassReadDto>> CreateAcademicClass(AcademicClassCreateDto academicClassCreateDto)
        {
            var createdAcademicClass = await _academicClassService.CreateAcademicClassAsync(academicClassCreateDto);
            
            return ApiResponse<AcademicClassReadDto>.SuccessResponse(createdAcademicClass);
        }
    }
}
