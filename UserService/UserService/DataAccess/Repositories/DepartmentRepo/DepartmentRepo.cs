using Microsoft.EntityFrameworkCore;
using UserService.Entities;

namespace UserService.DataAccess.Repositories.DepartmentRepo
{
    public class DepartmentRepo : IDepartmentRepo
    {
        private readonly AppDbContext _context;

        public DepartmentRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Department> GetDepartmentByIdAsync(Guid Id)
        {
            var result = await _context.Departments.FirstOrDefaultAsync(d => d.Id == Id);
            if (result == null)
            {
                throw new KeyNotFoundException("Department not found");
            }
            return result;
        }
    }
}
