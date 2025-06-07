using CourseService.Entities;
using CourseService.Utils.Filter;
using CourseService.Utils.Pagination;
using Microsoft.EntityFrameworkCore;

namespace CourseService.DataAccess.Repositories
{
    public class AcademicClassRepository : IAcademicClassRepository
    {
        private readonly AppDbContext _context;

        public AcademicClassRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AcademicClass> CreateAcademicClassAsync(AcademicClass academicClass)
        {
            var newAcademicClass = await _context.AcademicClasses.AddAsync(academicClass);
            await _context.SaveChangesAsync();
            return newAcademicClass.Entity;
        }        
        public async Task<AcademicClass?> GetAcademicClassByIdAsync(Guid id)
        {
            return await _context.AcademicClasses
                .Include(ac => ac.Course)
                .Include(ac => ac.Semester)
                .Include(ac => ac.ScheduleInDays)
                    .ThenInclude(s => s.Shift)
                .Include(ac => ac.ParentTheoryAcademicClass)
                    .ThenInclude(p => p.ScheduleInDays)
                        .ThenInclude(s => s.Shift)
                .Include(ac => ac.ChildPracticeAcademicClasses)
                    .ThenInclude(c => c.ScheduleInDays)
                        .ThenInclude(s => s.Shift)
                .FirstOrDefaultAsync(ac => ac.Id == id);
        }
        public async Task<List<AcademicClass>> GetAcademicClassesByCourseIdAsync(Guid courseId)
        {
            return await _context.AcademicClasses
                .Where(ac => ac.CourseId == courseId)
                .Include(ac => ac.Semester)
                .Include(ac => ac.ScheduleInDays)
                    .ThenInclude(s => s.Shift)
                .Include(ac => ac.ParentTheoryAcademicClass)
                    .ThenInclude(p => p.ScheduleInDays)
                        .ThenInclude(s => s.Shift)
                .Include(ac => ac.ChildPracticeAcademicClasses)
                    .ThenInclude(c => c.ScheduleInDays)
                        .ThenInclude(s => s.Shift)
                .ToListAsync();
        }        public async Task<List<AcademicClass>> GetAcademicClassesBySemesterIdAsync(Guid semesterId)
        {
            return await _context.AcademicClasses
                .Where(ac => ac.SemesterId == semesterId)
                .Include(ac => ac.Course)
                .Include(ac => ac.ScheduleInDays)
                    .ThenInclude(s => s.Shift)
                .Include(ac => ac.ParentTheoryAcademicClass)
                    .ThenInclude(p => p.ScheduleInDays)
                        .ThenInclude(s => s.Shift)
                .Include(ac => ac.ChildPracticeAcademicClasses)
                    .ThenInclude(c => c.ScheduleInDays)
                        .ThenInclude(s => s.Shift)
                .ToListAsync();
        }
        public async Task<PaginationResult<AcademicClass>> GetAllAcademicClassesPaginationAsync(
            Pagination pagination, AcademicClassFilterParams? filterParams, Order? order)
        {            
            IQueryable<AcademicClass> query = _context.AcademicClasses
                .Include(ac => ac.Course)
                .Include(ac => ac.Semester)
                .Include(ac => ac.ScheduleInDays)
                    .ThenInclude(s => s.Shift)
                .Include(ac => ac.ParentTheoryAcademicClass)
                    .ThenInclude(p => p.ScheduleInDays)
                        .ThenInclude(s => s.Shift)
                .Include(ac => ac.ChildPracticeAcademicClasses)
                    .ThenInclude(c => c.ScheduleInDays)
                        .ThenInclude(s => s.Shift);

            // Apply filters if provided
            if (filterParams != null)
            {
                query = ApplyFilters(query, filterParams);
            }

            // Apply ordering
            query = ApplySorting(query, order);

            // Apply pagination
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pagination.PageNumber - 1) * pagination.ItemsPerpage)
                .Take(pagination.ItemsPerpage)
                .ToListAsync();

            return new PaginationResult<AcademicClass>
            {
                Data = items,
                Total = totalCount,
                PageSize = pagination.ItemsPerpage,
                PageIndex = pagination.PageNumber
            };
        }        
        private IQueryable<AcademicClass> ApplyFilters(IQueryable<AcademicClass> queryable, AcademicClassFilterParams filterParams)
        {
            // Filter by name
            if (!string.IsNullOrWhiteSpace(filterParams.Name))
            {
                queryable = queryable.Where(ac => ac.Name.Contains(filterParams.Name));
            }

            // Filter by course ID
            if (filterParams.CourseId.HasValue && filterParams.CourseId != Guid.Empty)
            {
                queryable = queryable.Where(ac => ac.CourseId == filterParams.CourseId.Value);
            }

            // Filter by semester ID
            if (filterParams.SemesterId.HasValue && filterParams.SemesterId != Guid.Empty)
            {
                queryable = queryable.Where(ac => ac.SemesterId == filterParams.SemesterId.Value);
            }

            // Filter by registration status
            if (filterParams.IsRegistrable.HasValue)
            {
                queryable = queryable.Where(ac => ac.IsRegistrable == filterParams.IsRegistrable.Value);
            }

            // Filter by group number
            if (filterParams.GroupNumber.HasValue && filterParams.GroupNumber > 0)
            {
                queryable = queryable.Where(ac => ac.GroupNumber == filterParams.GroupNumber.Value);
            }

            // Filter by capacity range
            if (filterParams.MinCapacity.HasValue && filterParams.MinCapacity > 0)
            {
                queryable = queryable.Where(ac => ac.Capacity >= filterParams.MinCapacity.Value);
            }

            if (filterParams.MaxCapacity.HasValue && filterParams.MaxCapacity > 0)
            {
                queryable = queryable.Where(ac => ac.Capacity <= filterParams.MaxCapacity.Value);
            }

            // Filter by room ID
            if (filterParams.RoomId.HasValue && filterParams.RoomId != Guid.Empty)
            {
                queryable = queryable.Where(ac => ac.ScheduleInDays.Any(s => s.RoomId == filterParams.RoomId.Value));
            }

            // Filter by shift ID
            if (filterParams.ShiftId.HasValue && filterParams.ShiftId != Guid.Empty)
            {
                queryable = queryable.Where(ac => ac.ScheduleInDays.Any(s => s.ShiftId == filterParams.ShiftId.Value));
            }

            // Filter by specific schedule in day IDs
            if (filterParams.ScheduleInDayIds != null && filterParams.ScheduleInDayIds.Any())
            {
                queryable = queryable.Where(ac => ac.ScheduleInDays.Any(s => filterParams.ScheduleInDayIds.Contains(s.Id)));
            }

            return queryable;
        }

        private IQueryable<AcademicClass> ApplySorting(IQueryable<AcademicClass> queryable, Order? order)
        {
            if (order != null && !string.IsNullOrEmpty(order.By))
            {
                if (order.IsDesc)
                {
                    queryable = queryable.OrderByDescending(e => EF.Property<TrainingRoadmap>(e, order.By));
                }
                else
                {
                    queryable = queryable.OrderBy(e => EF.Property<TrainingRoadmap>(e, order.By));
                }
            }
            else
            {
                queryable = queryable.OrderByDescending(s => s.CreatedAt);
            }

            return queryable;
        }
        public async Task<AcademicClass> UpdateAcademicClassAsync(AcademicClass academicClass)
        {
            _context.AcademicClasses.Update(academicClass);
            await _context.SaveChangesAsync();
            return academicClass;
        }

        public async Task<List<AcademicClass>> GetAcademicClassesByIdsAsync(List<Guid> ids)
        {
            return await _context.AcademicClasses
                .Where(ac => ids.Contains(ac.Id))
                .Include(ac => ac.Course)
                .Include(ac => ac.Semester)
                .ToListAsync();
        }          public async Task<List<AcademicClass>> GetAcademicClassesForMajorAsync(Guid majorId)
        {
            return await _context.AcademicClasses
                .Where(ac => ac.IsRegistrable && 
                            (ac.Course.MajorIds.Contains(majorId) || ac.Course.IsOpenForAll))
                .Include(ac => ac.Course)
                .Include(ac => ac.Semester)
                .Include(ac => ac.ScheduleInDays)
                    .ThenInclude(s => s.Shift)
                .Include(ac => ac.ParentTheoryAcademicClass)
                    .ThenInclude(p => p.ScheduleInDays)
                        .ThenInclude(s => s.Shift)
                .Include(ac => ac.ChildPracticeAcademicClasses)
                    .ThenInclude(c => c.ScheduleInDays)
                        .ThenInclude(s => s.Shift)
                .ToListAsync();
        }

        public async Task<List<AcademicClass>> GetAcademicClassesForMajorAndBatchAsync(Guid majorId, Guid batchId)
        {
            // Get courses from TrainingRoadmapCourses
            var trainingRoadmapCourseIds = await _context.TrainingRoadmapCourses
                .Where(trc => trc.TrainingRoadmap.MajorId == majorId && 
                             trc.TrainingRoadmap.BatchIds.Contains(batchId) &&
                             trc.TrainingRoadmap.IsActive)
                .Select(trc => trc.CourseId)
                .ToListAsync();

            // Get courses from CoursesGroupSemesters
            var coursesGroupCourseIds = await _context.CoursesGroupSemesters
                .Where(cgs => cgs.TrainingRoadmap.MajorId == majorId &&
                             cgs.TrainingRoadmap.BatchIds.Contains(batchId) &&
                             cgs.TrainingRoadmap.IsActive)
                .SelectMany(cgs => cgs.CoursesGroup.CourseIds)
                .ToListAsync();

            // Combine both lists and get unique course IDs
            var allCourseIds = trainingRoadmapCourseIds.Concat(coursesGroupCourseIds).Distinct().ToList();

            // Get academic classes for these courses
            return await _context.AcademicClasses
                .Where(ac => ac.IsRegistrable && allCourseIds.Contains(ac.CourseId))
                .Include(ac => ac.Course)
                .Include(ac => ac.Semester)
                .Include(ac => ac.ScheduleInDays)
                    .ThenInclude(s => s.Shift)
                .Include(ac => ac.ParentTheoryAcademicClass)
                    .ThenInclude(p => p.ScheduleInDays)
                        .ThenInclude(s => s.Shift)
                .Include(ac => ac.ChildPracticeAcademicClasses)
                    .ThenInclude(c => c.ScheduleInDays)
                        .ThenInclude(s => s.Shift)
                .ToListAsync();
        }

        public async Task<List<AcademicClass>> GetAcademicClassesBySemesterWithSchedulesAsync(Guid semesterId)
        {
            return await _context.AcademicClasses
                .Where(ac => ac.SemesterId == semesterId)
                .Include(ac => ac.ScheduleInDays)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public IQueryable<AcademicClass> GetQuery()
        {
            return _context.AcademicClasses.AsQueryable();
        }
    }
}
