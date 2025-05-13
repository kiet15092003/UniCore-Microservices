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
        }

        public async Task<bool> GroupNameExistsInMajorAsync(string groupName, Guid majorId, Guid? excludeId = null)
        {
            var query = _context.CoursesGroups
                .Where(cg => cg.GroupName == groupName && cg.MajorId == majorId);
                
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
        }
    }
}