using CourseService.Business.Dtos.AcademicClass;
using CourseService.Business.Services;
using CourseService.Entities;
using Microsoft.AspNetCore.Mvc;
using CourseService.Middleware;

namespace CourseService.Controllers
{
    [Route("api/c/[controller]")]
    [ApiController]    
    public class AcademicClassesController : ControllerBase
    {
        private readonly IAcademicClassService _academicClassService;

        public AcademicClassesController(
            IAcademicClassService academicClassService)
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

        [HttpGet("major/{majorId}")]
        public async Task<ApiResponse<List<AcademicClassReadDto>>> GetAcademicClassesForMajor(Guid majorId)
        {
            var academicClasses = await _academicClassService.GetAcademicClassesForMajorAsync(majorId);

            return ApiResponse<List<AcademicClassReadDto>>.SuccessResponse(academicClasses);
        }

        [HttpPost]
        public async Task<ApiResponse<AcademicClassReadDto>> CreateAcademicClass(AcademicClassCreateDto academicClassCreateDto)
        {
            var createdAcademicClass = await _academicClassService.CreateAcademicClassAsync(academicClassCreateDto);

            return ApiResponse<AcademicClassReadDto>.SuccessResponse(createdAcademicClass);
        }        
        // The registration scheduling functionality has been consolidated into a single endpoint: /registration/schedule-with-times/// <summary>
        /// Sets the registration open and close times for multiple academic classes at once.
        /// The registration will automatically open and close at the specified times.
        /// </summary>
        [HttpPost("registration/schedule-with-times")]
        public async Task<ApiResponse<string>> ScheduleRegistrationWithTimes([FromBody] ClassRegistrationScheduleDto scheduleDto)
        {
            var success = await _academicClassService.ScheduleRegistrationAsync(scheduleDto);
            
            if (!success)
            {
                return ApiResponse<string>.ErrorResponse(new List<string> { "Failed to schedule registration - either invalid IDs provided or no classes found." });
            }

            string message = $"Registration scheduled for {scheduleDto.AcademicClassIds.Count} academic classes. " +
                             $"Opens at: {scheduleDto.RegistrationOpenTime:yyyy-MM-dd HH:mm:ss}, " +
                             $"Closes at: {scheduleDto.RegistrationCloseTime:yyyy-MM-dd HH:mm:ss}";
            
            return ApiResponse<string>.SuccessResponse(message);
        }
    }
}
