using CourseService.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourseService.DataAccess.Repositories
{
    public class ShiftRepository : IShiftRepository
    {
        private readonly AppDbContext _context;

        public ShiftRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Shift>> GetAllAsync()
        {
            return await _context.Shifts.ToListAsync();
        }

        public async Task<Shift> GetByIdAsync(Guid id)
        {
            return await _context.Shifts.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Shifts.AnyAsync(s => s.Id == id);
        }
    }
}
