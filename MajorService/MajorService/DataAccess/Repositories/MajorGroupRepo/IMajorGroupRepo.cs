using MajorService.Entities;
using MajorService.Utils.Filter;
using MajorService.Utils.Pagination;

namespace MajorService.DataAccess.Repositories.MajorGroupRepo
{
    public interface IMajorGroupRepo
    {
        Task<MajorGroup> GetMajorGroupByIdAsync(Guid id);
        Task<List<MajorGroup>> GetAllMajorGroupsAsync();
        Task<MajorGroup> CreateMajorGroupAsync(MajorGroup majorGroup);
        Task<bool> DeactivateMajorGroupAsync(Guid id);
        Task<PaginationResult<MajorGroup>> GetMajorGroupsByPaginationAsync(
            Pagination pagination,
            MajorGroupListFilterParams majorGroupListFilterParams,
            Order? order);
    }
}