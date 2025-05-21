using UserService.Business.Dtos.Guardian;
using UserService.DataAccess.Repositories.GuardianRepo;
using AutoMapper;
using UserService.Entities;

namespace UserService.Business.Services.GuardianService
{
    public class GuardianService : IGuardianService 
    {
        private readonly IGuardianRepo _guardianRepo;
        private readonly IMapper _mapper;

        public GuardianService(IGuardianRepo guardianRepo, IMapper mapper)
        {
            _guardianRepo = guardianRepo;
            _mapper = mapper;
        }

        public async Task<GuardianDto> CreateGuardianAsync(CreateGuardianDto createGuardianDto)
        {
            var guardian = _mapper.Map<Guardian>(createGuardianDto);
            var createdGuardian = await _guardianRepo.CreateGuardianAsync(guardian);
            return _mapper.Map<GuardianDto>(createdGuardian);
        }

        public async Task<GuardianDto> UpdateGuardianAsync(Guid id, UpdateGuardianDto updateGuardianDto)
        {
            var guardian = await _guardianRepo.GetGuardianByIdAsync(id);
            if (guardian == null)
                throw new KeyNotFoundException($"Guardian with ID {id} not found");

            _mapper.Map(updateGuardianDto, guardian);
            var updatedGuardian = await _guardianRepo.UpdateGuardianAsync(guardian);
            return _mapper.Map<GuardianDto>(updatedGuardian);
        }

        public async Task<List<GuardianDto>> UpdateGuardiansAsync(List<UpdateGuardianDto> updateGuardianDtos)
        {
            var updatedGuardians = new List<GuardianDto>();
            
            foreach (var dto in updateGuardianDtos)
            {
                if (dto.Id != Guid.Empty)
                {
                    var guardian = await _guardianRepo.GetGuardianByIdAsync(dto.Id);
                    if (guardian != null)
                    {
                        _mapper.Map(dto, guardian);
                        var updatedGuardian = await _guardianRepo.UpdateGuardianAsync(guardian);
                        updatedGuardians.Add(_mapper.Map<GuardianDto>(updatedGuardian));
                    }
                }
            }

            return updatedGuardians;
        }
    }
}
