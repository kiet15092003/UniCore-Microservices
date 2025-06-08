using EnrollmentService.Entities;
using EnrollmentService.Utils.Filter;
using EnrollmentService.Utils.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using CourseService.DataAccess;

namespace EnrollmentService.DataAccess.Repositories
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly AppDbContext _context;

        public EnrollmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Enrollment?> GetEnrollmentByIdAsync(Guid id)
        {
            return await _context.Enrollments
                .Include(e => e.StudentResults)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<PaginationResult<Enrollment>> GetAllEnrollmentsPaginationAsync(
            Pagination pagination,
            EnrollmentListFilterParams filterParams,
            Order? order)
        {
            var query = _context.Enrollments.AsQueryable();

            // Apply filters
            if (filterParams != null)
            {
                if (filterParams.StudentId.HasValue)
                {
                    query = query.Where(e => e.StudentId == filterParams.StudentId.Value);
                }

                if (filterParams.AcademicClassId.HasValue)
                {
                    query = query.Where(e => e.AcademicClassId == filterParams.AcademicClassId.Value);
                }

                if (filterParams.Status.HasValue)
                {
                    query = query.Where(e => e.Status == filterParams.Status.Value);
                }
            }

            // Apply ordering
            if (order != null)
            {
                switch (order.OrderBy.ToLower())
                {
                    case "createdat":
                        query = order.Direction.ToLower() == "asc" 
                            ? query.OrderBy(e => e.CreatedAt) 
                            : query.OrderByDescending(e => e.CreatedAt);
                        break;
                    case "status":
                        query = order.Direction.ToLower() == "asc" 
                            ? query.OrderBy(e => e.Status) 
                            : query.OrderByDescending(e => e.Status);
                        break;
                    default:
                        query = query.OrderByDescending(e => e.CreatedAt);
                        break;
                }
            }
            else
            {
                query = query.OrderByDescending(e => e.CreatedAt);
            }

            // Get total count
            var totalCount = await query.CountAsync();            // Apply pagination
            var items = await query
                .Skip((pagination.PageNumber - 1) * pagination.ItemsPerpage)
                .Take(pagination.ItemsPerpage)
                .Include(e => e.StudentResults)
                .ToListAsync();

            // Create pagination result
            return new PaginationResult<Enrollment>
            {
                Data = items,
                Total = totalCount,
                PageSize = pagination.ItemsPerpage,
                PageIndex = pagination.PageNumber - 1
            };
        }

        public async Task<Enrollment> CreateEnrollmentAsync(Enrollment enrollment)
        {
            enrollment.Id = Guid.NewGuid();
            enrollment.CreatedAt = DateTime.UtcNow;
            enrollment.UpdatedAt = DateTime.UtcNow;
            
            var newEnrollment = await _context.Enrollments.AddAsync(enrollment);
            await _context.SaveChangesAsync();
            return newEnrollment.Entity;
        }        public async Task<List<Enrollment>> CreateMultipleEnrollmentsAsync(List<Enrollment> enrollments)
        {
            foreach (var enrollment in enrollments)
            {
                enrollment.Id = Guid.NewGuid();
                enrollment.CreatedAt = DateTime.UtcNow;
                enrollment.UpdatedAt = DateTime.UtcNow;
            }

            await _context.Enrollments.AddRangeAsync(enrollments);
            await _context.SaveChangesAsync();
            return enrollments;
        }        public async Task<List<Enrollment>> CreateMultipleEnrollmentsWithTransactionAsync(List<Enrollment> enrollments)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var enrollment in enrollments)
                {
                    enrollment.Id = Guid.NewGuid();
                    enrollment.CreatedAt = DateTime.UtcNow;
                    enrollment.UpdatedAt = DateTime.UtcNow;
                }

                await _context.Enrollments.AddRangeAsync(enrollments);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return enrollments;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<Enrollment>> CreateMultipleEnrollmentsWithoutTransactionAsync(List<Enrollment> enrollments)
        {
            foreach (var enrollment in enrollments)
            {
                enrollment.Id = Guid.NewGuid();
                enrollment.CreatedAt = DateTime.UtcNow;
                enrollment.UpdatedAt = DateTime.UtcNow;
            }

            await _context.Enrollments.AddRangeAsync(enrollments);
            await _context.SaveChangesAsync();
            return enrollments;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }public async Task<bool> ExistsAsync(Guid studentId, Guid academicClassId)
        {
            return await _context.Enrollments
                .AnyAsync(e => e.StudentId == studentId && e.AcademicClassId == academicClassId);
        }        public async Task<int> GetEnrollmentCountByAcademicClassIdAsync(Guid academicClassId)
        {
            return await _context.Enrollments
                .Where(e => e.AcademicClassId == academicClassId)
                .CountAsync();
        }

        public async Task<int> GetEnrollmentCountByAcademicClassIdWithLockAsync(Guid academicClassId)
        {
            // Use SELECT FOR UPDATE equivalent in SQL Server (WITH (UPDLOCK, HOLDLOCK))
            //var count = await _context.Database
            //    .SqlQuery<int>($"SELECT COUNT(*) FROM Enrollments WITH (UPDLOCK, HOLDLOCK) WHERE AcademicClassId = {academicClassId}")
            //    .FirstAsync();

            //return count;
            var count = await _context.Database
               .SqlQuery<int>($"SELECT COUNT(*) AS Value FROM Enrollments WITH (UPDLOCK, HOLDLOCK) WHERE AcademicClassId = {academicClassId}")
               .FirstAsync();

            return count;
        }        public async Task<List<Enrollment>> GetEnrollmentsByStudentIdAsync(Guid studentId, Guid? semesterId = null)
        {
            var query = _context.Enrollments
                .Include(e => e.StudentResults)
                .Where(e => e.StudentId == studentId);

            // Note: Since we're in a microservices architecture and AcademicClass is in a different service,
            // we can't filter by semesterId at the database level. This filtering will be done at the service level.
            return await query.ToListAsync();
        }

        public async Task<bool> DeleteEnrollmentAsync(Guid id)
        {
            var enrollment = await _context.Enrollments
                .Include(e => e.StudentResults)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (enrollment == null)
            {
                return false;
            }

            // Remove related student results first
            if (enrollment.StudentResults != null && enrollment.StudentResults.Any())
            {
                _context.StudentResults.RemoveRange(enrollment.StudentResults);
            }

            // Remove the enrollment
            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
