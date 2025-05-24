using CourseService.CommunicationTypes.Grpc.GrpcClient;
using CourseService.Entities;
using CourseService.Utils.Filter;
using CourseService.Utils.Pagination;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace CourseService.DataAccess.Repositories
{
    public class TrainingRoadmapRepository : ITrainingRoadmapRepository
    {
        private readonly AppDbContext _context;
        private readonly GrpcMajorClientService _majorClient;
        private static readonly Random _random = new Random();

        public TrainingRoadmapRepository(AppDbContext context, GrpcMajorClientService majorClient)
        {
            _context = context;
            _majorClient = majorClient;
        }

        public async Task<TrainingRoadmap> CreateTrainingRoadmapAsync(TrainingRoadmap trainingRoadmap)
        {
            // Generate a 6-digit code
            trainingRoadmap.Code = GenerateSixDigitCode();
            
            await _context.TrainingRoadmaps.AddAsync(trainingRoadmap);
            await _context.SaveChangesAsync();
            
            return trainingRoadmap;
        }
          public async Task<TrainingRoadmap> GetTrainingRoadmapByIdAsync(Guid id)
        {
            return await _context.TrainingRoadmaps
                .Include(t => t.TrainingRoadmapCourses)
                    .ThenInclude(trc => trc.Course)
                .Include(t => t.CoursesGroupSemesters)
                    .ThenInclude(cgs => cgs.CoursesGroup)
                .FirstOrDefaultAsync(t => t.Id == id);
        }
        
        public async Task<TrainingRoadmap> UpdateTrainingRoadmapAsync(TrainingRoadmap trainingRoadmap)
        {
            var existingRoadmap = await _context.TrainingRoadmaps
                .FirstOrDefaultAsync(t => t.Id == trainingRoadmap.Id);
                
            if (existingRoadmap == null)
            {
                return null;
            }
            
            existingRoadmap.Name = trainingRoadmap.Name;
            existingRoadmap.Description = trainingRoadmap.Description;
            existingRoadmap.MajorId = trainingRoadmap.MajorId;
            existingRoadmap.StartYear = trainingRoadmap.StartYear;
            existingRoadmap.UpdatedAt = DateTime.Now;
            
            await _context.SaveChangesAsync();
            return existingRoadmap;
        }

        public async Task<PaginationResult<TrainingRoadmap>> GetAllTrainingRoadmapsPaginationAsync(
            Pagination pagination, 
            TrainingRoadmapFilterParams filterParams, 
            Order? order)
        {            
            var queryable = _context.TrainingRoadmaps
                .Include(t => t.TrainingRoadmapCourses)
                    .ThenInclude(trc => trc.Course)
                .Include(t => t.CoursesGroupSemesters)
                    .ThenInclude(cgs => cgs.CoursesGroup)
                .AsQueryable();
                
            queryable = ApplyFilters(queryable, filterParams);
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
                
                return new PaginationResult<TrainingRoadmap>
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
                
                return new PaginationResult<TrainingRoadmap>
                {
                    Data = result,
                    Total = total,
                    PageIndex = pagination.PageNumber,
                    PageSize = pagination.ItemsPerpage
                };
            }
        }        
        public async Task<TrainingRoadmap> AddTrainingRoadmapComponentsAsync(Guid trainingRoadmapId, List<CoursesGroupSemester> coursesGroupSemesters, List<TrainingRoadmapCourse> trainingRoadmapCourses)
        {
            var trainingRoadmap = await _context.TrainingRoadmaps
                .Include(t => t.TrainingRoadmapCourses)
                .Include(t => t.CoursesGroupSemesters)
                .FirstOrDefaultAsync(t => t.Id == trainingRoadmapId);
                
            if (trainingRoadmap == null)
            {
                throw new Exception($"Training roadmap with ID {trainingRoadmapId} not found");
            }
            
            // Remove existing CoursesGroupSemesters
            var existingCoursesGroupSemesters = await _context.Set<CoursesGroupSemester>()
                .Where(cgs => cgs.TrainingRoadmapId == trainingRoadmapId)
                .ToListAsync();
            if (existingCoursesGroupSemesters.Any())
            {
                _context.Set<CoursesGroupSemester>().RemoveRange(existingCoursesGroupSemesters);
            }
            
            // Remove existing TrainingRoadmapCourses
            var existingTrainingRoadmapCourses = await _context.TrainingRoadmapCourses
                .Where(trc => trc.TrainingRoadmapId == trainingRoadmapId)
                .ToListAsync();
            if (existingTrainingRoadmapCourses.Any())
            {
                _context.TrainingRoadmapCourses.RemoveRange(existingTrainingRoadmapCourses);
            }
            
            // Save changes to remove existing components
            await _context.SaveChangesAsync();
            
            // Add CoursesGroupSemesters if provided
            if (coursesGroupSemesters != null && coursesGroupSemesters.Any())
            {
                foreach (var cgs in coursesGroupSemesters)
                {
                    cgs.TrainingRoadmapId = trainingRoadmapId;
                    cgs.CreatedAt = DateTime.Now;
                }
                
                await _context.Set<CoursesGroupSemester>().AddRangeAsync(coursesGroupSemesters);
            }
            
            // Add TrainingRoadmapCourses if provided
            if (trainingRoadmapCourses != null && trainingRoadmapCourses.Any())
            {
                foreach (var trc in trainingRoadmapCourses)
                {
                    trc.TrainingRoadmapId = trainingRoadmapId;
                    trc.CreatedAt = DateTime.Now;
                }
                
                await _context.TrainingRoadmapCourses.AddRangeAsync(trainingRoadmapCourses);
            }
            
            await _context.SaveChangesAsync();
            
            // Reload the entity to get the updated data
            return await GetTrainingRoadmapByIdAsync(trainingRoadmapId);
        }

        private IQueryable<TrainingRoadmap> ApplyFilters(IQueryable<TrainingRoadmap> queryable, TrainingRoadmapFilterParams filterParams)
        {
            if (!string.IsNullOrWhiteSpace(filterParams.SearchQuery))
            {
                queryable = queryable.Where(t => t.Name.Contains(filterParams.SearchQuery));
            }

            if (filterParams.StartYear.HasValue)
            {
                queryable = queryable.Where(t => t.StartYear == filterParams.StartYear.Value);
            }

            if (!string.IsNullOrWhiteSpace(filterParams.Code))
            {
                queryable = queryable.Where(t => t.Code.Contains(filterParams.Code));
            }

            if (filterParams.MajorId.HasValue)
            {
                queryable = queryable.Where(t => t.MajorId == filterParams.MajorId.Value);
            }

            return queryable;
        }

        private async Task<IQueryable<TrainingRoadmap>> ApplySortingAsync(IQueryable<TrainingRoadmap> queryable, Order? order)
        {
            if (order != null && !string.IsNullOrEmpty(order.By))
            {
                if (order.By.Equals("Major", StringComparison.OrdinalIgnoreCase))
                {
                    // For sorting by major name, we need to materialize the roadmaps first
                    // and fetch major information via gRPC
                    var roadmaps = await queryable.ToListAsync();
                    
                    // Get all distinct majorIds
                    var majorIds = roadmaps.Where(r => r.MajorId != Guid.Empty)
                                         .Select(r => r.MajorId.ToString())
                                         .Distinct()
                                         .ToList();
                    
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
                    
                    // Sort roadmaps by major name
                    IEnumerable<TrainingRoadmap> sortedRoadmaps;
                    if (order.IsDesc)
                    {
                        sortedRoadmaps = roadmaps.OrderByDescending(r => 
                            r.MajorId != Guid.Empty && majorNameDictionary.TryGetValue(r.MajorId.ToString(), out string majorName) 
                                ? majorName 
                                : r.MajorId.ToString());
                    }
                    else
                    {
                        sortedRoadmaps = roadmaps.OrderBy(r => 
                            r.MajorId != Guid.Empty && majorNameDictionary.TryGetValue(r.MajorId.ToString(), out string majorName) 
                                ? majorName 
                                : r.MajorId.ToString());
                    }
                    
                    // Convert back to IQueryable
                    return sortedRoadmaps.AsQueryable();
                }
                else if (order.IsDesc)
                {
                    queryable = queryable.OrderByDescending(e => EF.Property<TrainingRoadmap>(e, order.By));
                }
                else
                {
                    queryable = queryable.OrderBy(e => EF.Property<TrainingRoadmap>(e, order.By));
                }
            }
            return queryable;
        }
        
        private string GenerateSixDigitCode()
        {
            // Generate a random 6-digit code
            int code = _random.Next(100000, 1000000); // 100000 to 999999
            return code.ToString();
        }
    }
}