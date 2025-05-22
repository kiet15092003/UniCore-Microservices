using CourseService.Entities;
using CourseService.Utils.Filter;
using CourseService.Utils.Pagination;
using Microsoft.EntityFrameworkCore;
using CourseService.CommunicationTypes.Grpc.GrpcClient;
using System.Collections.Concurrent;

namespace CourseService.DataAccess.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly AppDbContext _context;
        private readonly GrpcMajorClientService _majorClient;
        private static readonly Random _random = new Random();

        public CourseRepository(AppDbContext context, GrpcMajorClientService majorClient)
        {
            _context = context;
            _majorClient = majorClient;
        }        public async Task<Course> CreateCourseAsync(Course course)
        {
            // Always generate a 6-digit code, overriding any provided value
            course.Code = GenerateSixDigitCode();
            
            var newCourse = await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();
            return newCourse.Entity;
        }
        private string GenerateSixDigitCode()
        {
            // Generate a random 6-digit code
            int code = _random.Next(100000, 1000000); // 100000 to 999999
            return code.ToString();
        }
     
        private IQueryable<Course> ApplyFilters(IQueryable<Course> queryable, CourseListFilterParams courseListFilterParams)
        {
            if (!string.IsNullOrWhiteSpace(courseListFilterParams.Name))
            {
                queryable = queryable.Where(c => c.Name.Contains(courseListFilterParams.Name));
            }

            if (courseListFilterParams.IsActive.HasValue)
            {
                queryable = queryable.Where(c => c.IsActive == courseListFilterParams.IsActive.Value);
            }

            // Apply credit range filter
            if (courseListFilterParams.MinCredit.HasValue)
            {
                queryable = queryable.Where(c => c.Credit >= courseListFilterParams.MinCredit.Value);
            }

            if (courseListFilterParams.MaxCredit.HasValue)
            {
                queryable = queryable.Where(c => c.Credit <= courseListFilterParams.MaxCredit.Value);
            }

            // Filter by Course Code
            if (!string.IsNullOrWhiteSpace(courseListFilterParams.Code))
            {
                queryable = queryable.Where(c => c.Code.Contains(courseListFilterParams.Code));
            }

            // Filter by IsOpenForAll
            if (courseListFilterParams.IsOpenForAll.HasValue)
            {
                queryable = queryable.Where(c => c.IsOpenForAll == courseListFilterParams.IsOpenForAll.Value);
            }

            // Filter by IsRequired
            if (courseListFilterParams.IsRequired.HasValue)
            {
                queryable = queryable.Where(c => c.IsRequired == courseListFilterParams.IsRequired.Value);
            }

            // Filter by MajorId
            if (courseListFilterParams.MajorIds != null && courseListFilterParams.MajorIds.Length > 0)
            {
                queryable = queryable.Where(c => c.MajorIds != null &&
                    courseListFilterParams.MajorIds.Any(id => c.MajorIds.Contains(id)));
            }

            // Filter by PreCourseIds - courses that contain all specified prerequisite course IDs
            if (courseListFilterParams.PreCourseIds != null && courseListFilterParams.PreCourseIds.Length > 0)
            {
                queryable = queryable.Where(c => c.PreCourseIds != null && 
                    courseListFilterParams.PreCourseIds.All(id => c.PreCourseIds.Contains(id)));
            }

            // Filter by ParallelCourseIds - courses that contain all specified parallel course IDs
            if (courseListFilterParams.ParallelCourseIds != null && courseListFilterParams.ParallelCourseIds.Length > 0)
            {
                queryable = queryable.Where(c => c.ParallelCourseIds != null &&
                    courseListFilterParams.ParallelCourseIds.All(id => c.ParallelCourseIds.Contains(id)));
            }

            return queryable;
        }

        private async Task<IQueryable<Course>> ApplySortingAsync(IQueryable<Course> queryable, Order? order)
        {
            if (order != null && !string.IsNullOrEmpty(order.By))
            {
                if (order.IsDesc)
                {
                    queryable = queryable.OrderByDescending(e => EF.Property<CoursesGroup>(e, order.By));
                }
                else
                {
                    queryable = queryable.OrderBy(e => EF.Property<CoursesGroup>(e, order.By));
                }
            }
            return queryable;
        }

        public async Task<PaginationResult<Course>> GetAllCoursesPaginationAsync(Pagination pagination, CourseListFilterParams courseListFilterParams, Order? order)
        {
            var queryable = _context.Courses
                .Include(t => t.CourseMaterials)
                .Include(t => t.CourseCertificates)
                .AsQueryable();

            queryable = ApplyFilters(queryable, courseListFilterParams);
            queryable = queryable.OrderByDescending(e => EF.Property<DateTime>(e, "CreatedAt"));

            // Get total count before applying pagination
            int total = await queryable.CountAsync();

            // Apply sorting
            queryable = await ApplySortingAsync(queryable, order);

            var result = await queryable
                    .Skip((pagination.PageNumber - 1) * pagination.ItemsPerpage)
                    .Take(pagination.ItemsPerpage)
                    .ToListAsync();

            return new PaginationResult<Course>
            {
                Data = result,
                Total = total,
                PageIndex = pagination.PageNumber,
                PageSize = pagination.ItemsPerpage
            };
        }

        public async Task<Course> UpdateCourseAsync(Course course)
        {
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task<Course?> GetCourseByIdAsync(Guid id)
        {
            return await _context.Courses
                .Include(t => t.CourseMaterials)
                .Include(t => t.CourseCertificates)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
        
        public async Task<List<Course>> GetCoursesByIdsAsync(IEnumerable<Guid> ids)
        {
            return await _context.Courses
                .Include(t => t.CourseMaterials)
                .Include(t => t.CourseCertificates)
                .Where(c => ids.Contains(c.Id))
                .ToListAsync();
        }
    }
}
