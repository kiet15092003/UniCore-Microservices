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
    }
}
