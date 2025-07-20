using MajorService.Business.Dtos;
using MajorService.Business.Dtos.MajorGroup;
using MajorService.Entities;
using MajorService.Utils.Filter;
using MajorService.Utils.Pagination;

namespace MajorService.Business.Services.MajorGroupServices
{
    public interface IMajorGroupSvc
    {
        Task<List<MajorGroup>> GetAllMajorGroupsAsync();
        Task<MajorGroup> GetMajorGroupByIdAsync(Guid id);
        Task<MajorGroup> CreateMajorGroupAsync(MajorGroupCreateDto majorGroupCreateDto);
        Task<MajorGroup> CreateNewMajorGroupAsync(CreateNewMajorGroupDto dto);
        Task<MajorGroup> UpdateMajorGroupAsync(Guid id, UpdateMajorGroupDto dto);
        Task<bool> DeactivateMajorGroupAsync(DeactivateDto deactivateDto);
        Task<bool> ActivateMajorGroupAsync(ActivateDto activateDto);
        Task<MajorGroupListResponse> GetMajorGroupsByPaginationAsync(
            Pagination pagination, 
            MajorGroupListFilterParams majorGroupListFilterParams, 
            Order? order);
        Task<bool> DeleteMajorGroupAsync(Guid id);
    }
}