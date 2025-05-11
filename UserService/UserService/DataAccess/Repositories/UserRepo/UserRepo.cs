using Microsoft.EntityFrameworkCore;
using UserService.Entities;
using System.Text.Json;

namespace UserService.DataAccess.Repositories.UserRepo
{
    public class UserRepo : IUserRepo 
    {
        private readonly AppDbContext _context;

        public UserRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ApplicationUser>> AddRangeAsync(IEnumerable<ApplicationUser> users)
        {
            await _context.Users.AddRangeAsync(users);
            return users;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}