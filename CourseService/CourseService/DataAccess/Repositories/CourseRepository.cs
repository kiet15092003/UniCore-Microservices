using CourseService.Entities;
using CourseService.Utils.Filter;
using CourseService.Utils.Pagination;
using Microsoft.EntityFrameworkCore;

namespace CourseService.DataAccess.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly AppDbContext _context;

        public CourseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Course> CreateCourseAsync(Course course)
        {
            var newCourse = await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();
            return newCourse.Entity;
        }

        private IQueryable<Course> ApplyFilters(IQueryable<Course> queryable, CourseListFilterParams courseListFilterParams)
        {
            if (!string.IsNullOrWhiteSpace(courseListFilterParams.SearchQuery))
            {
                queryable = queryable.Where(c => c.Name.Contains(courseListFilterParams.SearchQuery));
            }

            return queryable;
        }
        private IQueryable<Course> ApplySorting(IQueryable<Course> queryable, Order? order)
        {
            if (order != null && !string.IsNullOrEmpty(order.By))
            {
                if (order.IsDesc)
                {
                    queryable = queryable.OrderByDescending(e => EF.Property<Course>(e, order.By));
                }
                else
                {
                    queryable = queryable.OrderBy(e => EF.Property<Course>(e, order.By));
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
            queryable = ApplySorting(queryable, order);

            int total = queryable.Count();

            queryable = queryable
                .Skip((pagination.PageNumber - 1) * pagination.ItemsPerpage)
                .Take(pagination.ItemsPerpage);

            List<Course> result = await queryable.ToListAsync();

            return new PaginationResult<Course>
            {
                Data = result,
                Total = total,
                PageIndex = pagination.PageNumber,
                PageSize = pagination.ItemsPerpage
            };
        }
    }
}
