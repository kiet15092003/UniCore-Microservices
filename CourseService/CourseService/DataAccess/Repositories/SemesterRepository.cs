using CourseService.Entities;
using CourseService.Utils.Filter;
using CourseService.Utils.Pagination;
using Microsoft.EntityFrameworkCore;

namespace CourseService.DataAccess.Repositories
{
    public class SemesterRepository : ISemesterRepository
    {
        private readonly AppDbContext _context;

        public SemesterRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Semester> CreateSemesterAsync(Semester semester)
        {
            var newSemester = await _context.Semesters.AddAsync(semester);
            await _context.SaveChangesAsync();
            return newSemester.Entity;
        }

        public async Task<PaginationResult<Semester>> GetAllSemestersPaginationAsync(
            Pagination pagination,
            SemesterFilterParams filterParams,
            Order? order)
        {
            var queryable = _context.Semesters.AsQueryable();
            
            // Apply filters
            queryable = ApplyFilters(queryable, filterParams);
            
            // Apply sorting
            queryable = ApplySorting(queryable, order);
            
            // Get total count
            var totalCount = await queryable.CountAsync();
            
            // Apply pagination
            var pagedData = await queryable
                .Skip((pagination.PageNumber - 1) * pagination.ItemsPerpage)
                .Take(pagination.ItemsPerpage)
                .ToListAsync();
                
            return new PaginationResult<Semester>
            {
                Data = pagedData,
                PageIndex = pagination.PageNumber,
                PageSize = pagination.ItemsPerpage,
                Total = totalCount,
            };
        }

        public async Task<Semester?> GetSemesterByIdAsync(Guid id)
        {
            return await _context.Semesters.FindAsync(id);
        }

        public async Task<Semester> UpdateSemesterAsync(Semester semester)
        {
            _context.Semesters.Update(semester);
            await _context.SaveChangesAsync();
            return semester;
        }

        public async Task<bool> SemesterExistsAsync(int semesterNumber, int year, Guid? excludeId = null)
        {
            var query = _context.Semesters.Where(s => 
                s.SemesterNumber == semesterNumber && 
                s.Year == year);
                
            if (excludeId.HasValue)
            {
                query = query.Where(s => s.Id != excludeId.Value);
            }
            
            return await query.AnyAsync();
        }

        private IQueryable<Semester> ApplyFilters(IQueryable<Semester> queryable, SemesterFilterParams filterParams)
        {
            if (filterParams.Year.HasValue)
            {
                queryable = queryable.Where(s => s.Year == filterParams.Year.Value);
            }

            if (filterParams.SemesterNumber.HasValue)
            {
                queryable = queryable.Where(s => s.SemesterNumber == filterParams.SemesterNumber.Value);
            }

            if (filterParams.IsActive.HasValue)
            {
                queryable = queryable.Where(s => s.IsActive == filterParams.IsActive.Value);
            }

            if (filterParams.StartDate.HasValue)
            {
                queryable = queryable.Where(s => s.StartDate >= filterParams.StartDate.Value);
            }

            if (filterParams.EndDate.HasValue)
            {
                queryable = queryable.Where(s => s.EndDate <= filterParams.EndDate.Value);
            }

            if (filterParams.NumberOfWeeks.HasValue)
            {
                queryable = queryable.Where(s => s.NumberOfWeeks == filterParams.NumberOfWeeks.Value);
            }

            return queryable;
        }

        private IQueryable<Semester> ApplySorting(IQueryable<Semester> queryable, Order? order)
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
    }
}
