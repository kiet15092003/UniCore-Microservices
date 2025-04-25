using MajorService.Business.Dtos.Major;
using MajorService.Entities;

namespace MajorService.DataAccess.Repositories.MajorRepo
{
    public interface IMajorRepo
    {
        Task<Major> GetMajorByIdAsync(Guid Id);
        Task<List<Major>> GetAllMajorAsync();
        Task<Major> CreateMajorAsync(Major major);
        Task<bool> DeactivateMajorAsync(Guid id);
    }
}
