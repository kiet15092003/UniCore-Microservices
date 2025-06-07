using CourseService.Entities;
using CourseService.Utils.Filter;
using CourseService.Utils.Pagination;
using Microsoft.EntityFrameworkCore;

namespace CourseService.DataAccess.Repositories
{
    public class CoursesGroupRepository : ICoursesGroupRepository
    {
        private readonly AppDbContext _context;

        public CoursesGroupRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CoursesGroup> CreateCoursesGroupAsync(CoursesGroup coursesGroup)
        {
            var newCoursesGroup = await _context.CoursesGroups.AddAsync(coursesGroup);
            await _context.SaveChangesAsync();
            return newCoursesGroup.Entity;
        }

        public async Task<IEnumerable<CoursesGroup>> CreateMultipleCoursesGroupsAsync(IEnumerable<CoursesGroup> coursesGroups)
        {
            await _context.CoursesGroups.AddRangeAsync(coursesGroups);
            await _context.SaveChangesAsync();
            return coursesGroups;
        }

        public async Task<CoursesGroup> GetCoursesGroupByIdAsync(Guid id)
        {
            return await _context.CoursesGroups
                .Include(cg => cg.CoursesGroupSemesters)
                .FirstOrDefaultAsync(cg => cg.Id == id);
        }        public async Task<bool> GroupNameExistsInMajorAsync(string groupName, Guid? majorId, Guid? excludeId = null)
        {
            var query = _context.CoursesGroups
                .Where(cg => cg.GroupName == groupName);
                
            // If majorId is provided, filter by it; otherwise check for groups without major (null)
            if (majorId.HasValue)
            {
                query = query.Where(cg => cg.MajorId == majorId.Value);
            }
            else
            {
                query = query.Where(cg => cg.MajorId == null);
            }
                
            if (excludeId.HasValue)
            {
                query = query.Where(cg => cg.Id != excludeId.Value);
            }
            
            return await query.AnyAsync();
        }

        public async Task<PaginationResult<CoursesGroup>> GetAllCoursesGroupsPaginationAsync(
            Pagination pagination, 
            Order? order)
        {
            var queryable = _context.CoursesGroups
                .Include(cg => cg.CoursesGroupSemesters)
                .AsQueryable();
            
            queryable = queryable.OrderByDescending(e => EF.Property<DateTime>(e, "CreatedAt"));
            
            // Get total count before applying pagination
            int total = await queryable.CountAsync();
            
            // Apply sorting
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

            // Apply pagination
            var result = await queryable
                .Skip((pagination.PageNumber - 1) * pagination.ItemsPerpage)
                .Take(pagination.ItemsPerpage)
                .ToListAsync();
            
            return new PaginationResult<CoursesGroup>
            {
                Data = result,
                Total = total,
                PageIndex = pagination.PageNumber,
                PageSize = pagination.ItemsPerpage
            };
        }

        public async Task<CoursesGroup> UpdateCoursesGroupAsync(CoursesGroup coursesGroup)
        {
            _context.CoursesGroups.Update(coursesGroup);
            await _context.SaveChangesAsync();
            return coursesGroup;
        }

        public async Task<IEnumerable<CoursesGroup>> GetCoursesGroupsByMajorIdAsync(Guid majorId)
        {
            return await _context.CoursesGroups
                .Include(cg => cg.CoursesGroupSemesters)
                .Where(cg => cg.MajorId == majorId)
                .OrderBy(cg => cg.GroupName)
                .ToListAsync();
        }        public async Task<IEnumerable<CoursesGroup>> GetCoursesGroupsWithAllCoursesOpenForAllAsync()
        {
            // First, get all course groups with their course IDs
            var allCoursesGroups = await _context.CoursesGroups
                .Include(cg => cg.CoursesGroupSemesters)
                .Where(cg => cg.CourseIds.Any()) // Only groups that have courses
                .ToListAsync();

            if (!allCoursesGroups.Any())
                return new List<CoursesGroup>();

            // Get all unique course IDs from all groups
            var allCourseIds = allCoursesGroups
                .SelectMany(cg => cg.CourseIds)
                .Distinct()
                .ToList();

            // Get all courses that are open for all in one query
            var coursesOpenForAll = await _context.Courses
                .Where(c => allCourseIds.Contains(c.Id) && c.IsOpenForAll == true)
                .Select(c => c.Id)
                .ToListAsync();

            // Filter groups where ALL courses are open for all
            var result = allCoursesGroups
                .Where(cg => cg.CourseIds.All(courseId => coursesOpenForAll.Contains(courseId)))
                .OrderBy(cg => cg.GroupName)
                .ToList();

            return result;
        }
    }
}