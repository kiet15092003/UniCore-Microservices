using UserService.Entities;

namespace UserService.DataAccess.Repositories.GuardianRepo
{
    public interface IGuardianRepo
    {
        Task<Guardian> CreateGuardianAsync(Guardian guardian);
        Task<Guardian> UpdateGuardianAsync(Guardian guardian);
        Task<Guardian> GetGuardianByIdAsync(Guid id);
    }
}