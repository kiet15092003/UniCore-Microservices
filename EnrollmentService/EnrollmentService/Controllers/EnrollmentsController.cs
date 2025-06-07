using EnrollmentService.Business.Dtos.Enrollment;
using EnrollmentService.Business.Services;
using EnrollmentService.Middleware;
using Microsoft.AspNetCore.Mvc;

namespace EnrollmentService.Controllers
{
    [Route("api/e/[controller]")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentsController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        [HttpGet("page")]
        public async Task<ApiResponse<EnrollmentListResponse>> GetByPagination([FromQuery] GetEnrollmentByPaginationParam param)
        {
            var result = await _enrollmentService.GetEnrollmentsByPagination(param.Pagination, param.Filter, param.Order);
            return ApiResponse<EnrollmentListResponse>.SuccessResponse(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<EnrollmentReadDto>>> GetById(Guid id)
        {
            var enrollment = await _enrollmentService.GetEnrollmentByIdAsync(id);
            if (enrollment == null)
            {
                return NotFound(ApiResponse<EnrollmentReadDto>.ErrorResponse(["Enrollment with ID {id} not found"]));
            }

            return ApiResponse<EnrollmentReadDto>.SuccessResponse(enrollment);
        }        
          
        [HttpPost("multiple")]
        public async Task<ActionResult<ApiResponse<List<EnrollmentReadDto>>>> CreateMultipleEnrollments([FromBody] MultipleEnrollmentCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<List<EnrollmentReadDto>>.ErrorResponse(["Invalid request data"]));
            }

            var result = await _enrollmentService.CreateMultipleEnrollmentsAsync(dto);
            return Ok(ApiResponse<List<EnrollmentReadDto>>.SuccessResponse(result));
        }        
        [HttpGet("check-exists")]
        public async Task<ActionResult<ApiResponse<EnrollmentExistsResponse>>> CheckEnrollmentExists([FromQuery] Guid studentId, [FromQuery] Guid academicClassId)
        {
            var exists = await _enrollmentService.CheckEnrollmentExistsAsync(studentId, academicClassId);
            var response = new EnrollmentExistsResponse { Exists = exists };
            return Ok(ApiResponse<EnrollmentExistsResponse>.SuccessResponse(response));
        }

        [HttpPost("check-multiple-exists")]
        public async Task<ActionResult<ApiResponse<CheckMultipleEnrollmentsResponse>>> CheckMultipleEnrollmentsExist([FromBody] CheckMultipleEnrollmentsRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<CheckMultipleEnrollmentsResponse>.ErrorResponse(["Invalid request data"]));
            }

            var result = await _enrollmentService.CheckMultipleEnrollmentsExistAsync(request);
            return Ok(ApiResponse<CheckMultipleEnrollmentsResponse>.SuccessResponse(result));
        }

        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<ApiResponse<List<EnrollmentReadDto>>>> GetEnrollmentsByStudentId(Guid studentId)
        {
            var enrollments = await _enrollmentService.GetEnrollmentsByStudentIdAsync(studentId);
            return Ok(ApiResponse<List<EnrollmentReadDto>>.SuccessResponse(enrollments));
        }
    }
}
