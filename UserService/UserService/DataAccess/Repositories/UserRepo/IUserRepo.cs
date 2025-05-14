using UserService.Entities;

namespace UserService.DataAccess.Repositories.UserRepo
{
    public interface IUserRepo
    {
        Task<IEnumerable<ApplicationUser>> AddRangeAsync(IEnumerable<ApplicationUser> users);
        Task SaveChangesAsync();
    }
}

