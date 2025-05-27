using Microsoft.EntityFrameworkCore;
using UserService.DataAccess;
using UserService.Entities;

namespace UserService.DataAccess.Repositories.GuardianRepo
{
    public class GuardianRepo : IGuardianRepo
    {
        private readonly AppDbContext _context;

        public GuardianRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Guardian> CreateGuardianAsync(Guardian guardian)
        {
            await _context.Guardians.AddAsync(guardian);
            await _context.SaveChangesAsync();
            return guardian;
        }

        public async Task<Guardian> UpdateGuardianAsync(Guardian guardian)
        {
            _context.Guardians.Update(guardian);
            await _context.SaveChangesAsync();
            return guardian;
        }

        public async Task<Guardian> GetGuardianByIdAsync(Guid id)
        {
            return await _context.Guardians.FindAsync(id);
        }
    }
}