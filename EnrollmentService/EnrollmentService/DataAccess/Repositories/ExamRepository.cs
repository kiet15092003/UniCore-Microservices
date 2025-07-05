using CourseService.DataAccess;
using EnrollmentService.Business.Dtos.Exam;
using EnrollmentService.Entities;
using EnrollmentService.Utils.Filter;
using EnrollmentService.Utils.Pagination;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentService.DataAccess.Repositories
{
    public class ExamRepository : IExamRepository
    {
        private readonly AppDbContext _context;

        public ExamRepository(AppDbContext context)
        {
            _context = context;
        }        public async Task<Exam?> GetExamByIdAsync(Guid id)
        {
            return await _context.Exams
                .Include(e => e.EnrollmentExams)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<Exam>> GetExamsByAcademicClassIdAsync(Guid academicClassId)
        {
            return await _context.Exams
                .Include(e => e.EnrollmentExams)
                .Where(e => e.AcademicClassId == academicClassId)
                .ToListAsync();
        }

        public async Task<Exam> CreateExamAsync(Exam exam)
        {
            exam.Id = Guid.NewGuid();
            exam.CreatedAt = DateTime.UtcNow;
            exam.UpdatedAt = DateTime.UtcNow;
            
            _context.Exams.Add(exam);
            await _context.SaveChangesAsync();
            return exam;
        }

        public async Task<Exam> UpdateExamAsync(Exam exam)
        {
            exam.UpdatedAt = DateTime.UtcNow;
            _context.Exams.Update(exam);
            await _context.SaveChangesAsync();
            return exam;
        }

        public async Task<bool> DeleteExamAsync(Guid id)
        {
            var exam = await GetExamByIdAsync(id);
            if (exam == null)
                return false;

            _context.Exams.Remove(exam);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Exam>> GetAllExamsAsync()
        {
            return await _context.Exams.ToListAsync();
        }

        public async Task<PaginationResult<Exam>> GetAllExamsPaginationAsync(Pagination pagination, ExamListFilterParams filterParams, Order? order)
        {
            var query = _context.Exams.AsQueryable();

            // Apply filters
            if (filterParams.AcademicClassId.HasValue)
            {
                query = query.Where(e => e.AcademicClassId == filterParams.AcademicClassId.Value);
            }

            if (filterParams.RoomId.HasValue)
            {
                query = query.Where(e => e.RoomId == filterParams.RoomId.Value);
            }

            if (filterParams.MinExamTime.HasValue)
            {
                query = query.Where(e => e.ExamTime >= filterParams.MinExamTime.Value);
            }

            if (filterParams.MaxExamTime.HasValue)
            {
                query = query.Where(e => e.ExamTime <= filterParams.MaxExamTime.Value);
            }

            // Apply ordering
            if (order != null && !string.IsNullOrEmpty(order.OrderBy))
            {
                switch (order.OrderBy.ToLower())
                {
                    case "examtime":
                        query = order.IsDesc ? query.OrderByDescending(e => e.ExamTime) : query.OrderBy(e => e.ExamTime);
                        break;
                    case "group":
                        query = order.IsDesc ? query.OrderByDescending(e => e.Group) : query.OrderBy(e => e.Group);
                        break;
                    case "type":
                        query = order.IsDesc ? query.OrderByDescending(e => e.Type) : query.OrderBy(e => e.Type);
                        break;
                    case "duration":
                        query = order.IsDesc ? query.OrderByDescending(e => e.Duration) : query.OrderBy(e => e.Duration);
                        break;
                    default:
                        query = order.IsDesc ? query.OrderByDescending(e => e.CreatedAt) : query.OrderBy(e => e.CreatedAt);
                        break;
                }
            }
            else
            {
                query = query.OrderByDescending(e => e.CreatedAt);
            }

            var total = await query.CountAsync();

            var data = await query
                .Include(e => e.EnrollmentExams)
                .Skip((pagination.PageNumber - 1) * pagination.ItemsPerpage)
                .Take(pagination.ItemsPerpage)
                .ToListAsync();            return new PaginationResult<Exam>
            {
                Data = data,
                Total = total,
                PageSize = pagination.ItemsPerpage,
                PageIndex = pagination.PageNumber
            };
        }

        public async Task<List<EnrollmentExam>> CreateEnrollmentExamsAsync(List<EnrollmentExam> enrollmentExams)
        {
            // Set IDs and timestamps for each enrollment exam
            foreach (var enrollmentExam in enrollmentExams)
            {
                enrollmentExam.Id = Guid.NewGuid();
                enrollmentExam.CreatedAt = DateTime.UtcNow;
                enrollmentExam.UpdatedAt = DateTime.UtcNow;
            }

            _context.EnrollmentExams.AddRange(enrollmentExams);
            await _context.SaveChangesAsync();
            return enrollmentExams;
        }
    }
}
