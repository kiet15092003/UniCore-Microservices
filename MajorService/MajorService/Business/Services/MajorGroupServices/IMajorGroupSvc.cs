using MajorService.Business.Dtos;
using MajorService.Business.Dtos.MajorGroup;
using MajorService.Entities;

namespace MajorService.Business.Services.MajorGroupServices
{
    public interface IMajorGroupSvc
    {
        Task<List<MajorGroup>> GetAllMajorGroupsAsync();
        Task<MajorGroup> GetMajorGroupByIdAsync(Guid id);
        Task<MajorGroup> CreateMajorGroupAsync(MajorGroupCreateDto majorGroupCreateDto);
        Task<bool> DeactivateMajorGroupAsync(DeactivateDto deactivateDto);
    }
}