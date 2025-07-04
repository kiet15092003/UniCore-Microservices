using EnrollmentService.Business.Dtos.Exam;
using EnrollmentService.Business.Services;
using EnrollmentService.Middleware;
using Microsoft.AspNetCore.Mvc;

namespace EnrollmentService.Controllers
{
    [Route("api/e/[controller]")]
    [ApiController]
    public class ExamsController : ControllerBase
    {
        private readonly IExamService _examService;

        public ExamsController(IExamService examService)
        {
            _examService = examService;
        }        
        [HttpGet]
        public async Task<ApiResponse<List<ExamReadDto>>> GetAllExams()
        {
            var exams = await _examService.GetAllExamsAsync();
            return ApiResponse<List<ExamReadDto>>.SuccessResponse(exams);
        }

        [HttpGet("{id}")]
        public async Task<ApiResponse<ExamReadDto>> GetById(Guid id)
        {
            var exam = await _examService.GetExamByIdAsync(id);
            if (exam == null)
            {
                return ApiResponse<ExamReadDto>.ErrorResponse([$"Exam with ID {id} not found"]);
            }

            return ApiResponse<ExamReadDto>.SuccessResponse(exam);
        }

        [HttpGet("academic-class/{academicClassId}")]
        public async Task<ApiResponse<List<ExamReadDto>>> GetByAcademicClassId(Guid academicClassId)
        {
            var exams = await _examService.GetExamsByAcademicClassIdAsync(academicClassId);
            return ApiResponse<List<ExamReadDto>>.SuccessResponse(exams);
        }

        [HttpPost]
        public async Task<ApiResponse<ExamReadDto>> CreateExam([FromBody] ExamCreateDto createDto)
        {
            try
            {
                var exam = await _examService.CreateExamAsync(createDto);
                return ApiResponse<ExamReadDto>.SuccessResponse(exam);
            }
            catch (ArgumentException ex)
            {
                return ApiResponse<ExamReadDto>.ErrorResponse([ex.Message]);
            }
            catch (Exception ex)
            {
                return ApiResponse<ExamReadDto>.ErrorResponse([$"Error creating exam: {ex.Message}"]);
            }
        }

        [HttpPut("{id}")]
        public async Task<ApiResponse<ExamReadDto>> UpdateExam(Guid id, [FromBody] ExamCreateDto updateDto)
        {
            try
            {
                var exam = await _examService.UpdateExamAsync(id, updateDto);
                return ApiResponse<ExamReadDto>.SuccessResponse(exam);
            }
            catch (ArgumentException ex)
            {
                return ApiResponse<ExamReadDto>.ErrorResponse([ex.Message]);
            }
            catch (Exception ex)
            {
                return ApiResponse<ExamReadDto>.ErrorResponse([$"Error updating exam: {ex.Message}"]);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ApiResponse<bool>> DeleteExam(Guid id)
        {
            try
            {
                var result = await _examService.DeleteExamAsync(id);
                if (!result)
                {
                    return ApiResponse<bool>.ErrorResponse([$"Exam with ID {id} not found"]);
                }

                return ApiResponse<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse([$"Error deleting exam: {ex.Message}"]);
            }
        }        
        
        [HttpGet("page")]
        public async Task<ApiResponse<ExamListResponse>> GetByPagination([FromQuery] GetExamByPaginationParam param)
        {
            var result = await _examService.GetExamsByPagination(param.Pagination, param.Filter, param.Order);
            return ApiResponse<ExamListResponse>.SuccessResponse(result);
        }

        [HttpPost("add-enrollment")]
        public async Task<ApiResponse<List<EnrollmentExamDto>>> AddEnrollmentToExam([FromBody] AddEnrollmentToExamDto addEnrollmentDto)
        {
            try
            {
                var result = await _examService.AddEnrollmentToExamAsync(addEnrollmentDto);
                return ApiResponse<List<EnrollmentExamDto>>.SuccessResponse(result);
            }
            catch (ArgumentException ex)
            {
                return ApiResponse<List<EnrollmentExamDto>>.ErrorResponse([ex.Message]);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<EnrollmentExamDto>>.ErrorResponse([$"Error adding enrollments to exam: {ex.Message}"]);
            }
        }
    }
}
