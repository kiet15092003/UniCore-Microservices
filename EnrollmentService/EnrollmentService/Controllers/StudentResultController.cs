using EnrollmentService.Business.Services;
using EnrollmentService.Business.Dtos.StudentResult;
using EnrollmentService.Utils.Filter;
using EnrollmentService.Utils.Pagination;
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
        public async Task<ActionResult<StudentResultDto>> GetStudentResultById(Guid id)
        {
            try
            {
                var result = await _studentResultService.GetStudentResultByIdAsync(id);
                if (result == null)
                {
                    return NotFound($"Student result with ID {id} not found");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting student result with ID {Id}", id);
                return StatusCode(500, "Internal server error");
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
        public async Task<ActionResult<PaginationResult<StudentResultDto>>> GetAllStudentResults(
            [FromQuery] Pagination pagination,
            [FromQuery] StudentResultListFilterParams filterParams,
            [FromQuery] Order? order)
        {
            try
            {
                var result = await _studentResultService.GetAllStudentResultsPaginationAsync(
                    pagination, filterParams, order);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting paginated student results");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update student result score
        /// </summary>
        /// <param name="id">Student result ID</param>
        /// <param name="updateDto">Update data</param>
        /// <returns>Updated student result</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<StudentResultDto>> UpdateStudentResult(
            Guid id, 
            [FromBody] UpdateStudentResultDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _studentResultService.UpdateStudentResultAsync(id, updateDto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating student result with ID {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Delete student result
        /// </summary>
        /// <param name="id">Student result ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteStudentResult(Guid id)
        {
            try
            {
                var result = await _studentResultService.DeleteStudentResultAsync(id);
                if (!result)
                {
                    return NotFound($"Student result with ID {id} not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting student result with ID {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get student results by enrollment ID
        /// </summary>
        /// <param name="enrollmentId">Enrollment ID</param>
        /// <returns>List of student results for the enrollment</returns>
        [HttpGet("enrollment/{enrollmentId}")]
        public async Task<ActionResult<List<StudentResultDto>>> GetStudentResultsByEnrollmentId(Guid enrollmentId)
        {
            try
            {
                var results = await _studentResultService.GetStudentResultsByEnrollmentIdAsync(enrollmentId);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting student results for enrollment ID {EnrollmentId}", enrollmentId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get student results by multiple enrollment IDs
        /// </summary>
        /// <param name="enrollmentIds">List of enrollment IDs</param>
        /// <returns>List of student results for the enrollments</returns>
        [HttpPost("enrollments")]
        public async Task<ActionResult<List<StudentResultDto>>> GetStudentResultsByEnrollmentIds(
            [FromBody] List<Guid> enrollmentIds)
        {
            try
            {
                if (enrollmentIds == null || !enrollmentIds.Any())
                {
                    return BadRequest("Enrollment IDs list cannot be empty");
                }

                var results = await _studentResultService.GetStudentResultsByEnrollmentIdsAsync(enrollmentIds);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting student results for enrollment IDs {EnrollmentIds}", enrollmentIds);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get student results by class ID
        /// </summary>
        /// <param name="classId">Academic class ID</param>
        /// <returns>List of student results for the class</returns>
        [HttpGet("class/{classId}")]
        public async Task<ActionResult<List<StudentResultDto>>> GetStudentResultsByClassId(Guid classId)
        {
            try
            {
                var results = await _studentResultService.GetStudentResultsByClassIdAsync(classId);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting student results for class ID {ClassId}", classId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Import student scores from Excel file for a specific class
        /// </summary>
        /// <param name="classId">Academic class ID</param>
        /// <param name="excelFile">Excel file containing student scores</param>
        /// <returns>Import result with success and error details</returns>
        [HttpPost("import-scores/{classId}")]
        public async Task<ActionResult<ImportScoreResultDto>> ImportScoresFromExcel(Guid classId, IFormFile excelFile)
        {
            try
            {
                if (excelFile == null || excelFile.Length == 0)
                {
                    return BadRequest("Excel file is required");
                }

                if (!excelFile.FileName.EndsWith(".xlsx") && !excelFile.FileName.EndsWith(".xls"))
                {
                    return BadRequest("File must be an Excel file (.xlsx or .xls)");
                }

                var result = await _studentResultService.ImportScoresFromExcelAsync(classId, excelFile);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while importing scores from Excel for class ID {ClassId}", classId);
                return StatusCode(500, "Internal server error");
            }
        }
    }
} 