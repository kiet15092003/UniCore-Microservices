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
    }
}
