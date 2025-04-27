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

        public CourseRepository(AppDbContext context, GrpcMajorClientService majorClient)
        {
            _context = context;
            _majorClient = majorClient;
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

            // Filter by IsRegistrable
            if (courseListFilterParams.IsRegistrable.HasValue)
            {
                queryable = queryable.Where(c => c.IsRegistrable == courseListFilterParams.IsRegistrable.Value);
            }

            // Filter by IsRequired
            if (courseListFilterParams.IsRequired.HasValue)
            {
                queryable = queryable.Where(c => c.IsRequired == courseListFilterParams.IsRequired.Value);
            }

            // Filter by MajorId
            if (courseListFilterParams.MajorId.HasValue)
            {
                queryable = queryable.Where(c => c.MajorId == courseListFilterParams.MajorId.Value);
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
                if (order.By.Equals("Major", StringComparison.OrdinalIgnoreCase))
                {
                    // For sorting by major name, we need to materialize the courses first
                    // and fetch major information via gRPC
                    var courses = await queryable.ToListAsync();
                    
                    // Get all distinct majorIds
                    var majorIds = courses.Select(c => c.MajorId.ToString()).Distinct().ToList();
                    
                    // Create a dictionary to map majorId to major name
                    var majorNameDictionary = new ConcurrentDictionary<string, string>();
                    
                    // Fetch major information for all majors in parallel
                    var tasks = majorIds.Select(async majorId => {
                        var majorResponse = await _majorClient.GetMajorByIdAsync(majorId);
                        if (majorResponse.Success)
                        {
                            majorNameDictionary.TryAdd(majorId, majorResponse.Data.Name);
                        }
                        else
                        {
                            // If we can't get the name, use the ID as fallback
                            majorNameDictionary.TryAdd(majorId, majorId);
                        }
                    });
                    
                    await Task.WhenAll(tasks);
                    
                    // Sort courses by major name
                    IEnumerable<Course> sortedCourses;
                    if (order.IsDesc)
                    {
                        sortedCourses = courses.OrderByDescending(c => 
                            majorNameDictionary.TryGetValue(c.MajorId.ToString(), out string majorName) 
                                ? majorName 
                                : c.MajorId.ToString());
                    }
                    else
                    {
                        sortedCourses = courses.OrderBy(c => 
                            majorNameDictionary.TryGetValue(c.MajorId.ToString(), out string majorName) 
                                ? majorName 
                                : c.MajorId.ToString());
                    }
                    
                    // Convert back to IQueryable
                    return sortedCourses.AsQueryable();
                }
                else if (order.IsDesc)
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
            
            // Get total count before applying pagination
            int total = await queryable.CountAsync();
            
            // Apply sorting (may convert to in-memory sorting for "Major" field)
            queryable = await ApplySortingAsync(queryable, order);

            // If sorting was done in-memory (for Major field), we need to apply pagination manually
            if (order?.By?.Equals("Major", StringComparison.OrdinalIgnoreCase) == true)
            {
                var sortedList = queryable.Skip((pagination.PageNumber - 1) * pagination.ItemsPerpage)
                                         .Take(pagination.ItemsPerpage)
                                         .ToList();
                
                return new PaginationResult<Course>
                {
                    Data = sortedList,
                    Total = total,
                    PageIndex = pagination.PageNumber,
                    PageSize = pagination.ItemsPerpage
                };
            }
            else
            {
                // Normal EF pagination
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
        }

        public async Task<Course> UpdateCourseAsync(Course course)
        {
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task<Course> GetCourseByIdAsync(Guid id)
        {
            return await _context.Courses
                .Include(t => t.CourseMaterials)
                .Include(t => t.CourseCertificates)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
