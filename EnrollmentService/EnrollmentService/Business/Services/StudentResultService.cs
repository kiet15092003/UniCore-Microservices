using AutoMapper;
using EnrollmentService.DataAccess.Repositories;
using EnrollmentService.Business.Dtos.StudentResult;
using EnrollmentService.Utils.Filter;
using EnrollmentService.Utils.Pagination;
using Microsoft.Extensions.Logging;

namespace EnrollmentService.Business.Services
{
    public class StudentResultService : IStudentResultService
    {
        private readonly IStudentResultRepository _studentResultRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<StudentResultService> _logger;

        public StudentResultService(
            IStudentResultRepository studentResultRepository,
            IMapper mapper,
            ILogger<StudentResultService> logger)
        {
            _studentResultRepository = studentResultRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<StudentResultDto?> GetStudentResultByIdAsync(Guid id)
        {
            try
            {
                var studentResult = await _studentResultRepository.GetStudentResultByIdAsync(id);
                return studentResult != null ? _mapper.Map<StudentResultDto>(studentResult) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting student result with ID {Id}", id);
                throw;
            }
        }

        public async Task<PaginationResult<StudentResultDto>> GetAllStudentResultsPaginationAsync(
            Pagination pagination,
            StudentResultListFilterParams filterParams,
            Order? order)
        {
            try
            {
                var result = await _studentResultRepository.GetAllStudentResultsPaginationAsync(
                    pagination, filterParams, order);

                return new PaginationResult<StudentResultDto>
                {
                    Data = _mapper.Map<List<StudentResultDto>>(result.Data),
                    Total = result.Total,
                    PageSize = result.PageSize,
                    PageIndex = result.PageIndex
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting paginated student results");
                throw;
            }
        }

        public async Task<StudentResultDto> UpdateStudentResultAsync(Guid id, UpdateStudentResultDto updateDto)
        {
            try
            {
                var studentResult = await _studentResultRepository.GetStudentResultByIdAsync(id);
                if (studentResult == null)
                {
                    throw new KeyNotFoundException($"Student result with ID {id} not found");
                }

                _mapper.Map(updateDto, studentResult);
                var updatedResult = await _studentResultRepository.UpdateStudentResultAsync(studentResult);
                return _mapper.Map<StudentResultDto>(updatedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating student result with ID {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteStudentResultAsync(Guid id)
        {
            try
            {
                return await _studentResultRepository.DeleteStudentResultAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting student result with ID {Id}", id);
                throw;
            }
        }

        public async Task<List<StudentResultDto>> GetStudentResultsByEnrollmentIdAsync(Guid enrollmentId)
        {
            try
            {
                var results = await _studentResultRepository.GetStudentResultsByEnrollmentIdAsync(enrollmentId);
                return _mapper.Map<List<StudentResultDto>>(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting student results for enrollment ID {EnrollmentId}", enrollmentId);
                throw;
            }
        }

        public async Task<List<StudentResultDto>> GetStudentResultsByEnrollmentIdsAsync(List<Guid> enrollmentIds)
        {
            try
            {
                var results = await _studentResultRepository.GetStudentResultsByEnrollmentIdsAsync(enrollmentIds);
                return _mapper.Map<List<StudentResultDto>>(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting student results for enrollment IDs {EnrollmentIds}", enrollmentIds);
                throw;
            }
        }

        public async Task<List<StudentResultDto>> GetStudentResultsByClassIdAsync(Guid classId)
        {
            try
            {
                var results = await _studentResultRepository.GetStudentResultsByClassIdAsync(classId);
                return _mapper.Map<List<StudentResultDto>>(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting student results for class ID {ClassId}", classId);
                throw;
            }
        }
    }
} 