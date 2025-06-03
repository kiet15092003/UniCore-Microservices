using EnrollmentService.Entities;
using EnrollmentService.Utils.Filter;
using EnrollmentService.Utils.Pagination;
using Microsoft.EntityFrameworkCore;
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
                .Include(e => e.studentResults)
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
            var totalCount = await query.CountAsync();

            // Apply pagination
            var items = await query
                .Skip((pagination.Page - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .Include(e => e.studentResults)
                .ToListAsync();

            // Create pagination result
            return new PaginationResult<Enrollment>
            {
                Items = items,
                Page = pagination.Page,
                PageSize = pagination.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pagination.PageSize)
            };
        }
    }
}
