using MajorService.Business.Dtos;
using MajorService.Business.Dtos.Major;
using MajorService.Utils.Filter;
using MajorService.Utils.Pagination;

namespace MajorService.Business.Services.MajorServices
{
    public interface IMajorSvc
    {
        Task<List<MajorReadDto>> GetAllMajorAsync();   
        Task<MajorReadDto> CreateMajorAsync(MajorCreateDto majorCreateDto);
        Task<MajorReadDto> CreateNewMajorAsync(CreateNewMajorDto dto);
        Task<bool> DeactivateMajorAsync(DeactivateDto deactivateDto);
        Task<bool> ActivateMajorAsync(ActivateDto activateDto);
        Task<MajorListResponse> GetMajorsByPaginationAsync(Pagination pagination, MajorListFilterParams majorListFilterParams, Order? order);
        Task<bool> DeleteMajorAsync(Guid id);
    }
}
