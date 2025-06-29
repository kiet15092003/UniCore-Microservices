using EnrollmentService.Business.Services;
using EnrollmentService.Business.Dtos.StudentResult;
using EnrollmentService.Utils.Filter;
using EnrollmentService.Utils.Pagination;
using EnrollmentService.Middleware;
using Microsoft.AspNetCore.Mvc;

namespace EnrollmentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentResultController : ControllerBase
    {
        private readonly IStudentResultService _studentResultService;
        private readonly ILogger<StudentResultController> _logger;

        public StudentResultController(
            IStudentResultService studentResultService,
            ILogger<StudentResultController> logger)
        {
            _studentResultService = studentResultService;
            _logger = logger;
        }

        /// <summary>
        /// Get student result by ID
        /// </summary>
        /// <param name="id">Student result ID</param>
        /// <returns>Student result details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<StudentResultDto>>> GetStudentResultById(Guid id)
        {
            try
            {
                var result = await _studentResultService.GetStudentResultByIdAsync(id);
                if (result == null)
                {
                    return NotFound(ApiResponse<StudentResultDto>.ErrorResponse([$"Student result with ID {id} not found"]));
                }
                return ApiResponse<StudentResultDto>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting student result with ID {Id}", id);
                return StatusCode(500, ApiResponse<StudentResultDto>.ErrorResponse(["Internal server error"]));
            }
        }

        /// <summary>
        /// Get all student results with pagination, filtering and sorting
        /// </summary>
        /// <param name="pagination">Pagination parameters</param>
        /// <param name="filterParams">Filter parameters</param>
        /// <param name="order">Sorting parameters</param>
        /// <returns>Paginated list of student results</returns>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PaginationResult<StudentResultDto>>>> GetAllStudentResults(
            [FromQuery] Pagination pagination,
            [FromQuery] StudentResultListFilterParams filterParams,
            [FromQuery] Order? order)
        {
            try
            {
                var result = await _studentResultService.GetAllStudentResultsPaginationAsync(
                    pagination, filterParams, order);
                return ApiResponse<PaginationResult<StudentResultDto>>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting paginated student results");
                return StatusCode(500, ApiResponse<PaginationResult<StudentResultDto>>.ErrorResponse(["Internal server error"]));
            }
        }

        /// <summary>
        /// Update student result score
        /// </summary>
        /// <param name="id">Student result ID</param>
        /// <param name="updateDto">Update data</param>
        /// <returns>Updated student result</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<StudentResultDto>>> UpdateStudentResult(
            Guid id, 
            [FromBody] UpdateStudentResultDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<StudentResultDto>.ErrorResponse(["Invalid request data"]));
                }

                var result = await _studentResultService.UpdateStudentResultAsync(id, updateDto);
                return ApiResponse<StudentResultDto>.SuccessResponse(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<StudentResultDto>.ErrorResponse([ex.Message]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating student result with ID {Id}", id);
                return StatusCode(500, ApiResponse<StudentResultDto>.ErrorResponse(["Internal server error"]));
            }
        }

        /// <summary>
        /// Delete student result
        /// </summary>
        /// <param name="id">Student result ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        public async Task<ApiResponse<bool>> DeleteStudentResult(Guid id)
        {
            try
            {
                var result = await _studentResultService.DeleteStudentResultAsync(id);
                if (result)
                {
                    return ApiResponse<bool>.SuccessResponse(true);
                }
                return ApiResponse<bool>.ErrorResponse([$"Student result with ID {id} not found"]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting student result with ID {Id}", id);
                return ApiResponse<bool>.ErrorResponse(["Internal server error"]);
            }
        }

        /// <summary>
        /// Get student results by enrollment ID
        /// </summary>
        /// <param name="enrollmentId">Enrollment ID</param>
        /// <returns>List of student results for the enrollment</returns>
        [HttpGet("enrollment/{enrollmentId}")]
        public async Task<ActionResult<ApiResponse<List<StudentResultDto>>>> GetStudentResultsByEnrollmentId(Guid enrollmentId)
        {
            try
            {
                var results = await _studentResultService.GetStudentResultsByEnrollmentIdAsync(enrollmentId);
                return ApiResponse<List<StudentResultDto>>.SuccessResponse(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting student results for enrollment ID {EnrollmentId}", enrollmentId);
                return StatusCode(500, ApiResponse<List<StudentResultDto>>.ErrorResponse(["Internal server error"]));
            }
        }

        /// <summary>
        /// Get student results by multiple enrollment IDs
        /// </summary>
        /// <param name="enrollmentIds">List of enrollment IDs</param>
        /// <returns>List of student results for the enrollments</returns>
        [HttpPost("enrollments")]
        public async Task<ActionResult<ApiResponse<List<StudentResultDto>>>> GetStudentResultsByEnrollmentIds(
            [FromBody] List<Guid> enrollmentIds)
        {
            try
            {
                if (enrollmentIds == null || !enrollmentIds.Any())
                {
                    return BadRequest(ApiResponse<List<StudentResultDto>>.ErrorResponse(["Enrollment IDs list cannot be empty"]));
                }

                var results = await _studentResultService.GetStudentResultsByEnrollmentIdsAsync(enrollmentIds);
                return ApiResponse<List<StudentResultDto>>.SuccessResponse(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting student results for enrollment IDs {EnrollmentIds}", enrollmentIds);
                return StatusCode(500, ApiResponse<List<StudentResultDto>>.ErrorResponse(["Internal server error"]));
            }
        }

        /// <summary>
        /// Get student results by class ID
        /// </summary>
        /// <param name="classId">Academic class ID</param>
        /// <returns>List of student results for the class</returns>
        [HttpGet("class/{classId}")]
        public async Task<ActionResult<ApiResponse<List<StudentResultDto>>>> GetStudentResultsByClassId(Guid classId)
        {
            try
            {
                var results = await _studentResultService.GetStudentResultsByClassIdAsync(classId);
                return ApiResponse<List<StudentResultDto>>.SuccessResponse(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting student results for class ID {ClassId}", classId);
                return StatusCode(500, ApiResponse<List<StudentResultDto>>.ErrorResponse(["Internal server error"]));
            }
        }

        /// <summary>
        /// Import student scores from Excel file for a specific class
        /// </summary>
        /// <param name="classId">Academic class ID</param>
        /// <param name="excelFile">Excel file containing student scores</param>
        /// <returns>Import success status</returns>
        [HttpPost("import-scores/{classId}")]
        public async Task<ApiResponse<bool>> ImportScoresFromExcel(Guid classId, IFormFile excelFile)
        {
            try
            {
                if (excelFile == null || excelFile.Length == 0)
                {
                    return ApiResponse<bool>.ErrorResponse(["Excel file is required"]);
                }

                if (!excelFile.FileName.EndsWith(".xlsx") && !excelFile.FileName.EndsWith(".xls"))
                {
                    return ApiResponse<bool>.ErrorResponse(["File must be an Excel file (.xlsx or .xls)"]);
                }

                var result = await _studentResultService.ImportScoresFromExcelAsync(classId, excelFile);
                
                // Return true if there are any successful imports, false if all failed
                var hasSuccess = result.SuccessCount > 0;
                return ApiResponse<bool>.SuccessResponse(hasSuccess);
            }
            catch (ArgumentException ex)
            {
                return ApiResponse<bool>.ErrorResponse([ex.Message]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while importing scores from Excel for class ID {ClassId}", classId);
                return ApiResponse<bool>.ErrorResponse(["Internal server error"]);
            }
        }
    }
} 