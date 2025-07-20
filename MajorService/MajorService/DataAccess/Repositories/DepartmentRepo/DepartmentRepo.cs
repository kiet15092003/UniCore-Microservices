using Microsoft.EntityFrameworkCore;
using MajorService.Entities;
using MajorService.Utils.Filter;
using MajorService.Utils.Pagination;

namespace MajorService.DataAccess.Repositories.DepartmentRepo
{
    public class DepartmentRepo : IDepartmentRepo
    {
        private readonly AppDbContext _context;
        
        public DepartmentRepo(AppDbContext context)
        {
            _context = context;
        }
        
        public async Task<Department> GetDepartmentByIdAsync(Guid id)
        {
            var result = await _context.Departments.FirstOrDefaultAsync(d => d.Id == id);
            if (result == null)
            {
                throw new KeyNotFoundException("Department not found");
            }
            return result;
        }
        
        public async Task<List<Department>> GetAllDepartmentsAsync()
        {
            return await _context.Departments.Where(d => d.IsActive).ToListAsync();
        }
        
        public async Task<Department> CreateDepartmentAsync(Department department)
        {
            department.Id = Guid.NewGuid();
            department.CreatedAt = DateTime.UtcNow;
            department.UpdatedAt = DateTime.UtcNow;
            
            await _context.Departments.AddAsync(department);
            await _context.SaveChangesAsync();
            
            return department;
        }

        public async Task<Department> UpdateDepartmentAsync(Department department)
        {
            var existingDepartment = await _context.Departments.FindAsync(department.Id);
            if (existingDepartment == null)
            {
                throw new KeyNotFoundException("Department not found");
            }
            
            existingDepartment.Name = department.Name;
            existingDepartment.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            
            return existingDepartment;
        }
          public async Task<bool> DeactivateDepartmentAsync(Guid id)
        {
            var department = await _context.Departments.FirstOrDefaultAsync(d => d.Id == id);
            if (department == null)
            {
                return false;
            }
            
            department.IsActive = false;
            department.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<bool> ActivateDepartmentAsync(Guid id)
        {
            var department = await _context.Departments.FirstOrDefaultAsync(d => d.Id == id);
            if (department == null)
            {
                return false;
            }
            
            department.IsActive = true;
            department.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<PaginationResult<Department>> GetDepartmentsByPaginationAsync(
            Pagination pagination,
            DepartmentListFilterParams departmentListFilterParams,
            Order? order)
        {
            var query = _context.Departments.AsQueryable();
            
            // Apply filters
            query = ApplyFilters(query, departmentListFilterParams);
            
            // Get total count before paging
            var totalCount = await query.CountAsync();
            
            // Apply sorting
            query = await ApplySortingAsync(query, order);
            
            // Apply pagination
            var items = await query
                .Skip((pagination.PageNumber - 1) * pagination.ItemsPerpage)
                .Take(pagination.ItemsPerpage)
                .ToListAsync();
                
            return new PaginationResult<Department>
            {
                Data = items,
                Total = totalCount,
                PageSize = pagination.ItemsPerpage,
                PageIndex = pagination.PageNumber
            };
        }
        
        private IQueryable<Department> ApplyFilters(IQueryable<Department> queryable, DepartmentListFilterParams departmentListFilterParams)
        {
            // Apply filters
            if (!string.IsNullOrEmpty(departmentListFilterParams.Name))
            {
                queryable = queryable.Where(d => d.Name.Contains(departmentListFilterParams.Name));
            }
            
            if (!string.IsNullOrEmpty(departmentListFilterParams.Code))
            {
                queryable = queryable.Where(d => d.Code.Contains(departmentListFilterParams.Code));
            }
            
            if (departmentListFilterParams.IsActive.HasValue)
            {
                queryable = queryable.Where(d => d.IsActive == departmentListFilterParams.IsActive.Value);
            }
            
            return queryable;
        }
          private async Task<IQueryable<Department>> ApplySortingAsync(IQueryable<Department> queryable, Order? order)
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
        
        public async Task<bool> IsDepartmentNameExistsAsync(string name)
        {
            return await _context.Departments
                .AnyAsync(d => d.Name.ToLower() == name.ToLower());
        }

        public async Task<bool> IsDepartmentNameExistsForOtherAsync(Guid id, string name)
        {
            return await _context.Departments
                .AnyAsync(d => d.Id != id && d.Name.ToLower() == name.ToLower());
        }
        
        public async Task<string> GenerateUniqueCodeAsync()
        {
            string code;
            bool codeExists;
            
            do
            {
                // Generate code with format UNIDEPART-[6 digits]
                Random random = new Random();
                string digits = random.Next(100000, 1000000).ToString();
                code = $"{digits}";
                
                // Check if code exists
                codeExists = await _context.Departments.AnyAsync(d => d.Code == code);
            } 
            while (codeExists);
            
            return code;
        }

        public async Task<bool> DeleteDepartmentAsync(Guid id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return false;
            }

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<MajorGroup>> GetMajorGroupsByDepartmentIdAsync(Guid departmentId)
        {
            return await _context.MajorGroups
                .Where(mg => mg.DepartmentId == departmentId)
                .ToListAsync();
        }
    }
}