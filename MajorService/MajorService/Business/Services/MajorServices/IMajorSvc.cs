using MajorService.Business.Dtos;
using MajorService.Business.Dtos.Major;

namespace MajorService.Business.Services.MajorServices
{
    public interface IMajorSvc
    {
        Task<List<MajorReadDto>> GetAllMajorAsync();   
        Task<MajorReadDto> CreateMajorAsync(MajorCreateDto majorCreateDto);
        Task<bool> DeactivateMajorAsync(DeactivateDto deactivateDto);
    }
}
