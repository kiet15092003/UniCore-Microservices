using MajorService.Business.Dtos.Major;

namespace MajorService.Business.Services.MajorServices
{
    public interface IMajorSvc
    {
        Task<List<MajorReadDto>> GetAllMajorAsync();   
    }
}
