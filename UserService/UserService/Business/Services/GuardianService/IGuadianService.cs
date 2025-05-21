using UserService.Business.Dtos.Guardian;

namespace UserService.Business.Services.GuardianService
{
    public interface IGuardianService
    {
        Task<GuardianDto> CreateGuardianAsync(CreateGuardianDto createGuardianDto);
        Task<GuardianDto> UpdateGuardianAsync(Guid id, UpdateGuardianDto updateGuardianDto);
        Task<List<GuardianDto>> UpdateGuardiansAsync(List<UpdateGuardianDto> updateGuardianDtos);
    }
}

