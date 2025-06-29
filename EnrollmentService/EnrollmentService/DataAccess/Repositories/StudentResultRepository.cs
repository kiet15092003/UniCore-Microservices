using EnrollmentService.Entities;
using EnrollmentService.Utils.Filter;
using EnrollmentService.Utils.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CourseService.DataAccess;

namespace EnrollmentService.DataAccess.Repositories
{
    public class StudentResultRepository : IStudentResultRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<StudentResultRepository> _logger;

        public StudentResultRepository(AppDbContext context, ILogger<StudentResultRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<StudentResult?> GetStudentResultByIdAsync(Guid id)
        {
            return await _context.StudentResults
                .Include(sr => sr.ScoreType)
                .Include(sr => sr.Enrollment)
                .FirstOrDefaultAsync(sr => sr.Id == id);
        }

        public async Task<PaginationResult<StudentResult>> GetAllStudentResultsPaginationAsync(
            Pagination pagination,
            StudentResultListFilterParams filterParams,
            Order? order)
        {
            var query = _context.StudentResults
                .Include(sr => sr.ScoreType)
                .Include(sr => sr.Enrollment)
                .AsQueryable();

            // Apply filters
            if (filterParams != null)
            {
                if (filterParams.EnrollmentId.HasValue)
                {
                    query = query.Where(sr => sr.EnrollmentId == filterParams.EnrollmentId.Value);
                }

                if (filterParams.ScoreTypeId.HasValue)
                {
                    query = query.Where(sr => sr.ScoreTypeId == filterParams.ScoreTypeId.Value);
                }

                if (filterParams.MinScore.HasValue)
                {
                    query = query.Where(sr => sr.Score >= filterParams.MinScore.Value);
                }

                if (filterParams.MaxScore.HasValue)
                {
                    query = query.Where(sr => sr.Score <= filterParams.MaxScore.Value);
                }

                if (filterParams.FromDate.HasValue)
                {
                    query = query.Where(sr => sr.CreatedAt >= filterParams.FromDate.Value);
                }

                if (filterParams.ToDate.HasValue)
                {
                    var toDate = filterParams.ToDate.Value.Date.AddDays(1).AddTicks(-1);
                    query = query.Where(sr => sr.CreatedAt <= toDate);
                }
            }

            // Apply ordering
            if (order != null)
            {
                switch (order.OrderBy.ToLower())
                {
                    case "score":
                        query = order.IsDesc 
                            ? query.OrderByDescending(sr => sr.Score) 
                            : query.OrderBy(sr => sr.Score);
                        break;
                    case "createdat":
                        query = order.IsDesc 
                            ? query.OrderByDescending(sr => sr.CreatedAt) 
                            : query.OrderBy(sr => sr.CreatedAt);
                        break;
                    default:
                        query = order.IsDesc 
                            ? query.OrderByDescending(sr => sr.CreatedAt)
                            : query.OrderBy(sr => sr.CreatedAt);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(sr => sr.CreatedAt);
            }

            // Get total count
            var totalCount = await query.CountAsync();

            // Apply pagination
            var items = await query
                .Skip((pagination.PageNumber - 1) * pagination.ItemsPerpage)
                .Take(pagination.ItemsPerpage)
                .ToListAsync();

            return new PaginationResult<StudentResult>
            {
                Data = items,
                Total = totalCount,
                PageSize = pagination.ItemsPerpage,
                PageIndex = pagination.PageNumber - 1
            };
        }

        public async Task<StudentResult> UpdateStudentResultAsync(StudentResult studentResult)
        {
            studentResult.UpdatedAt = DateTime.UtcNow;
            _context.StudentResults.Update(studentResult);
            await _context.SaveChangesAsync();
            return studentResult;
        }

        public async Task<bool> DeleteStudentResultAsync(Guid id)
        {
            var studentResult = await _context.StudentResults
                .Include(sr => sr.AttendanceInDays)
                .FirstOrDefaultAsync(sr => sr.Id == id);

            if (studentResult == null)
            {
                return false;
            }

            // Remove related attendance records first
            if (studentResult.AttendanceInDays != null && studentResult.AttendanceInDays.Any())
            {
                _context.AttendanceInDays.RemoveRange(studentResult.AttendanceInDays);
            }

            _context.StudentResults.Remove(studentResult);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<StudentResult>> GetStudentResultsByEnrollmentIdAsync(Guid enrollmentId)
        {
            return await _context.StudentResults
                .Include(sr => sr.ScoreType)
                .Where(sr => sr.EnrollmentId == enrollmentId)
                .ToListAsync();
        }

        public async Task<List<StudentResult>> GetStudentResultsByEnrollmentIdsAsync(List<Guid> enrollmentIds)
        {
            return await _context.StudentResults
                .Include(sr => sr.ScoreType)
                .Where(sr => enrollmentIds.Contains(sr.EnrollmentId))
                .ToListAsync();
        }

        public async Task<List<StudentResult>> GetStudentResultsByClassIdAsync(Guid classId)
        {
            return await _context.StudentResults
                .Include(sr => sr.ScoreType)
                .Include(sr => sr.Enrollment)
                .Where(sr => sr.Enrollment.AcademicClassId == classId)
                .ToListAsync();
        }
    }
} 