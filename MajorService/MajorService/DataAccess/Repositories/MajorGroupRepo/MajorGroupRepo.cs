using Microsoft.EntityFrameworkCore;
using MajorService.Entities;
using MajorService.Utils.Filter;
using MajorService.Utils.Pagination;

namespace MajorService.DataAccess.Repositories.MajorGroupRepo
{
    public class MajorGroupRepo : IMajorGroupRepo
    {
        private readonly AppDbContext _context;
        
        public MajorGroupRepo(AppDbContext context)
        {
            _context = context;
        }
        
        public async Task<MajorGroup> GetMajorGroupByIdAsync(Guid id)
        {
            var result = await _context.MajorGroups.FirstOrDefaultAsync(mg => mg.Id == id);
            if (result == null)
            {
                throw new KeyNotFoundException("MajorGroup not found");
            }
            return result;
        }
        
        public async Task<List<MajorGroup>> GetAllMajorGroupsAsync()
        {
            return await _context.MajorGroups.ToListAsync();
        }
        
        public async Task<MajorGroup> CreateMajorGroupAsync(MajorGroup majorGroup)
        {
            majorGroup.Id = Guid.NewGuid();
            majorGroup.CreatedAt = DateTime.UtcNow;
            majorGroup.UpdatedAt = DateTime.UtcNow;
            
            await _context.MajorGroups.AddAsync(majorGroup);
            await _context.SaveChangesAsync();
            
            return majorGroup;
        }
        
        public async Task<bool> DeactivateMajorGroupAsync(Guid id)
        {
            var majorGroup = await _context.MajorGroups.FirstOrDefaultAsync(mg => mg.Id == id);
            if (majorGroup == null)
            {
                return false;
            }
            
            majorGroup.IsActive = false;
            majorGroup.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<PaginationResult<MajorGroup>> GetMajorGroupsByPaginationAsync(
            Pagination pagination,
            MajorGroupListFilterParams majorGroupListFilterParams,
            Order? order)
        {
            var query = _context.MajorGroups
                .Include(mg => mg.Department)
                .AsQueryable();
            
            // Apply filters
            query = ApplyFilters(query, majorGroupListFilterParams);
            
            // Get total count before paging
            var totalCount = await query.CountAsync();
            
            // Apply sorting
            query = await ApplySortingAsync(query, order);
            
            // Apply pagination
            var items = await query
                .Skip((pagination.PageNumber - 1) * pagination.ItemsPerpage)
                .Take(pagination.ItemsPerpage)
                .ToListAsync();
                
            return new PaginationResult<MajorGroup>
            {
                Data = items,
                Total = totalCount,
                PageSize = pagination.ItemsPerpage,
                PageIndex = pagination.PageNumber
            };
        }
        
        private IQueryable<MajorGroup> ApplyFilters(IQueryable<MajorGroup> queryable, MajorGroupListFilterParams majorGroupListFilterParams)
        {
            // Apply filters
            if (!string.IsNullOrEmpty(majorGroupListFilterParams.Name))
            {
                queryable = queryable.Where(mg => mg.Name.Contains(majorGroupListFilterParams.Name));
            }
            
            if (majorGroupListFilterParams.IsActive.HasValue)
            {
                queryable = queryable.Where(mg => mg.IsActive == majorGroupListFilterParams.IsActive.Value);
            }
            
            if (majorGroupListFilterParams.DepartmentId.HasValue)
            {
                queryable = queryable.Where(mg => mg.DepartmentId == majorGroupListFilterParams.DepartmentId.Value);
            }
            
            return queryable;
        }
        
        private async Task<IQueryable<MajorGroup>> ApplySortingAsync(IQueryable<MajorGroup> queryable, Order? order)
        {
            if (order != null && !string.IsNullOrEmpty(order.By))
            {
                if (order.IsDesc)
                {
                    queryable = queryable.OrderByDescending(e => EF.Property<object>(e, order.By));
                }
                else
                {
                    queryable = queryable.OrderBy(e => EF.Property<object>(e, order.By));
                }
            }
            return queryable;
        }
    }
}