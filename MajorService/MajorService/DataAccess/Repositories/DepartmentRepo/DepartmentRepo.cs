using Microsoft.EntityFrameworkCore;
using MajorService.Entities;

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
            return await _context.Departments.ToListAsync();
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
    }
}