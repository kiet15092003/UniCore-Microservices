using Microsoft.EntityFrameworkCore;
using MajorService.Entities;

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
            var result = await _context.Majors.FirstOrDefaultAsync(m => m.Id == Id);
            if (result == null)
            {
                throw new KeyNotFoundException("Major not found");
            }
            return result;
        }
        public async Task<List<Major>> GetAllMajorAsync()
        {
            return await _context.Majors.ToListAsync();
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
    }
}
