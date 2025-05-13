using Microsoft.EntityFrameworkCore;
using MajorService.Entities;
using MajorService.Business.Dtos.Major;
using MajorService.Utils.Pagination;
using MajorService.Utils.Filter;
using System;
using System.Threading.Tasks;

namespace MajorService.DataAccess.Repositories.MajorRepo
{
    public class MajorRepo : IMajorRepo
    {
        private readonly AppDbContext _context;
        public MajorRepo(AppDbContext context)
        {
            _context = context;
        }
    public async Task<Major> GetMajorByIdAsync(Guid Id)
        {
            var result = await _context.Majors
                .Include(m => m.MajorGroup)
                .ThenInclude(mg => mg.Department)
                .FirstOrDefaultAsync(m => m.Id == Id);
            if (result == null)
            {
                throw new KeyNotFoundException("Major not found");
            }
            return result;
        }        public async Task<List<Major>> GetAllMajorAsync()
        {
            return await _context.Majors
                .Include(m => m.MajorGroup)
                .ThenInclude(mg => mg.Department)
                .ToListAsync();
        }
        
        public async Task<Major> CreateMajorAsync(Major major)
        {
            major.Id = Guid.NewGuid();
            major.CreatedAt = DateTime.UtcNow;
            major.UpdatedAt = DateTime.UtcNow;
            
            await _context.Majors.AddAsync(major);
            await _context.SaveChangesAsync();
            
            return major;
        }
        
        public async Task<bool> DeactivateMajorAsync(Guid id)
        {
            var major = await _context.Majors.FirstOrDefaultAsync(m => m.Id == id);
            if (major == null)
            {
                return false;
            }
            
            major.IsActive = false;
            major.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return true;
        }
          private IQueryable<Major> ApplyFilters(IQueryable<Major> queryable, MajorListFilterParams majorListFilterParams)
        {
            // Apply filters
            if (!string.IsNullOrEmpty(majorListFilterParams.Name))
            {
                queryable = queryable.Where(m => m.Name.Contains(majorListFilterParams.Name));
            }
            
            if (!string.IsNullOrEmpty(majorListFilterParams.Code))
            {
                queryable = queryable.Where(m => m.Code.Contains(majorListFilterParams.Code));
            }
            
            if (majorListFilterParams.IsActive.HasValue)
            {
                queryable = queryable.Where(m => m.IsActive == majorListFilterParams.IsActive.Value);
            }
            
            if (majorListFilterParams.MajorGroupId.HasValue)
            {
                queryable = queryable.Where(m => m.MajorGroupId == majorListFilterParams.MajorGroupId.Value);
            }
            
            return queryable;
        }
          private IQueryable<Major> ApplySorting(IQueryable<Major> queryable, Order? order)
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
          public async Task<PaginationResult<Major>> GetMajorsByPaginationAsync(
            Pagination pagination,
            MajorListFilterParams majorListFilterParams,
            Order? order)
        {
            var query = _context.Majors
                .Include(m => m.MajorGroup)
                .ThenInclude(mg => mg.Department)
                .AsQueryable();
            
            // Apply filters
            query = ApplyFilters(query, majorListFilterParams);
            
            // Get total count before paging
            var totalCount = await query.CountAsync();
            
            // Apply sorting
            query = ApplySorting(query, order);
            
            // Apply pagination
            var items = await query
                .Skip((pagination.PageNumber - 1) * pagination.ItemsPerpage)
                .Take(pagination.ItemsPerpage)
                .ToListAsync();              return new PaginationResult<Major>
            {
                Data = items,
                Total = totalCount,
                PageSize = pagination.ItemsPerpage,
                PageIndex = pagination.PageNumber
            };
        }
          
        public async Task<bool> IsMajorNameExistsAsync(string name)
        {
            return await _context.Majors
                .AnyAsync(m => m.Name.ToLower() == name.ToLower());
        }
        
        public async Task<string> GenerateUniqueCodeAsync()
        {
            string code;
            bool codeExists;
            
            do
            {
                // Generate code with 6 random digits
                Random random = new Random();
                string digits = random.Next(100000, 1000000).ToString();
                code = digits;
                
                // Check if code exists
                codeExists = await _context.Majors.AnyAsync(m => m.Code == code);
            } 
            while (codeExists);
            
            return code;
        }
    }
}
