using Microsoft.EntityFrameworkCore;
using MajorService.Entities;

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
    }
}