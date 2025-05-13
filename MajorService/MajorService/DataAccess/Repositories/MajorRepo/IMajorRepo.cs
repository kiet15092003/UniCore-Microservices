using MajorService.Business.Dtos.Major;
using MajorService.Entities;
using MajorService.Utils.Filter;
using MajorService.Utils.Pagination;

namespace MajorService.DataAccess.Repositories.MajorRepo
{
    public interface IMajorRepo
    {
        Task<Major> GetMajorByIdAsync(Guid Id);
        Task<List<Major>> GetAllMajorAsync();
        Task<Major> CreateMajorAsync(Major major);
        Task<bool> DeactivateMajorAsync(Guid id);
        Task<bool> ActivateMajorAsync(Guid id);
        Task<PaginationResult<Major>> GetMajorsByPaginationAsync(
            Pagination pagination,
            MajorListFilterParams majorListFilterParams,
            Order? order);
        Task<bool> IsMajorNameExistsAsync(string name);
        Task<string> GenerateUniqueCodeAsync();
    }
}
