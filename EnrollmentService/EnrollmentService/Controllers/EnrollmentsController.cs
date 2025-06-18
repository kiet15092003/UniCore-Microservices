using EnrollmentService.Business.Dtos.Enrollment;
using EnrollmentService.Business.Services;
using EnrollmentService.Middleware;
using EnrollmentService.Utils.Exceptions;
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
        public async Task<ApiResponse<List<EnrollmentReadDto>>> CreateMultipleEnrollments([FromBody] MultipleEnrollmentCreateDto dto)
        {
            var result = await _enrollmentService.CreateMultipleEnrollmentsAsync(dto);
            return ApiResponse<List<EnrollmentReadDto>>.SuccessResponse(result);
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
        public async Task<ActionResult<ApiResponse<List<EnrollmentReadDto>>>> GetEnrollmentsByStudentId(Guid studentId, [FromQuery] Guid? semesterId = null)
        {
            var enrollments = await _enrollmentService.GetEnrollmentsByStudentIdAsync(studentId, semesterId);
            return Ok(ApiResponse<List<EnrollmentReadDto>>.SuccessResponse(enrollments));
        }

        [HttpDelete("{id}")]
        public async Task<ApiResponse<bool>> DeleteEnrollment(Guid id)
        {
            try
            {
                var result = await _enrollmentService.DeleteEnrollmentAsync(id);
                if (result)
                {
                    return ApiResponse<bool>.SuccessResponse(true);
                }
                return ApiResponse<bool>.ErrorResponse(["Enrollment with the specified ID not found"]);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse([$"Error deleting enrollment: {ex.Message}"]);
            }
        }

        [HttpPut("approve-by-class/{classId}")]
        public async Task<ApiResponse<bool>> ApproveEnrollmentsByAcademicClassId(Guid classId)
        {
            try
            {
                var result = await _enrollmentService.ApproveEnrollmentsByAcademicClassIdAsync(classId);
                return ApiResponse<bool>.SuccessResponse(result > 0);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse([$"Error approving enrollments: {ex.Message}"]);
            }
        }

        [HttpPut("reject-by-class/{classId}")]
        public async Task<ApiResponse<bool>> RejectEnrollmentsByAcademicClassId(Guid classId)
        {
            try
            {
                var result = await _enrollmentService.RejectEnrollmentsByAcademicClassIdAsync(classId);
                return ApiResponse<bool>.SuccessResponse(result > 0);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse([$"Error rejecting enrollments: {ex.Message}"]);
            }
        }

        [HttpGet("class/{classId}")]
        public async Task<ActionResult<ApiResponse<List<EnrollmentReadDto>>>> GetEnrollmentsByClassId(Guid classId)
        {
            try
            {
                var enrollments = await _enrollmentService.GetEnrollmentsByAcademicClassIdAsync(classId);
                return Ok(ApiResponse<List<EnrollmentReadDto>>.SuccessResponse(enrollments));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<EnrollmentReadDto>>.ErrorResponse([$"Error retrieving enrollments: {ex.Message}"]));
            }        
        }        
        [HttpPut("move-to-class")]
        public async Task<ApiResponse<int>> MoveEnrollmentsToNewClass([FromBody] MoveEnrollmentsDto moveDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ApiResponse<int>.ErrorResponse(["Invalid request data"]);
                }

                if (moveDto.EnrollmentIds == null || !moveDto.EnrollmentIds.Any())
                {
                    return ApiResponse<int>.ErrorResponse(["At least one enrollment ID is required"]);
                }

                var result = await _enrollmentService.MoveEnrollmentsToNewClassAsync(moveDto.EnrollmentIds, moveDto.ToClassId);
                return ApiResponse<int>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse([$"Error moving enrollments: {ex.Message}"]);
            }
        }        
        [HttpPost("check-class-conflict")]
        public async Task<ApiResponse<CheckClassConflictResponse>> CheckClassConflict([FromBody] CheckClassConflictRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ApiResponse<CheckClassConflictResponse>.ErrorResponse(["Invalid request data"]);
                }

                if (request.EnrollmentIds == null || !request.EnrollmentIds.Any())
                {
                    return ApiResponse<CheckClassConflictResponse>.ErrorResponse(["At least one enrollment ID is required"]);
                }

                var result = await _enrollmentService.CheckClassConflictAsync(request);
                return ApiResponse<CheckClassConflictResponse>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                return ApiResponse<CheckClassConflictResponse>.ErrorResponse([$"Error checking class conflicts: {ex.Message}"]);
            }
        }
        [HttpPost("start-by-class/{classId}")]
        public async Task<ApiResponse<bool>> StartEnrollmentsByAcademicClassId(Guid classId)
        {
            try
            {
                var result = await _enrollmentService.StartEnrollmentsByAcademicClassIdAsync(classId);
                return ApiResponse<bool>.SuccessResponse(result > 0);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse([$"Error starting enrollments: {ex.Message}"]);
            }
        }
        [HttpPut("bulk-change-status")]
        public async Task<ApiResponse<int>> BulkChangeEnrollmentStatus([FromBody] BulkStatusChangeDto bulkStatusChangeDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ApiResponse<int>.ErrorResponse(["Invalid request data"]);
                }

                if (bulkStatusChangeDto.ClassIds == null || !bulkStatusChangeDto.ClassIds.Any())
                {
                    return ApiResponse<int>.ErrorResponse(["At least one class ID is required"]);
                }

                var updatedCount = await _enrollmentService.BulkChangeEnrollmentStatusAsync(bulkStatusChangeDto);
                return ApiResponse<int>.SuccessResponse(updatedCount);
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse([$"Error changing enrollment status: {ex.Message}"]);
            }
        }
    }
}
