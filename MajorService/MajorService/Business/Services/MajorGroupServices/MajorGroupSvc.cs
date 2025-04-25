using MajorService.Business.Dtos;
using MajorService.Business.Dtos.MajorGroup;
using MajorService.DataAccess.Repositories.MajorGroupRepo;
using MajorService.Entities;

namespace MajorService.Business.Services.MajorGroupServices
{
    public class MajorGroupSvc : IMajorGroupSvc
    {
        private readonly IMajorGroupRepo _majorGroupRepo;
        
        public MajorGroupSvc(IMajorGroupRepo majorGroupRepo)
        {
            _majorGroupRepo = majorGroupRepo;
        }
        
        public async Task<List<MajorGroup>> GetAllMajorGroupsAsync()
        {
            return await _majorGroupRepo.GetAllMajorGroupsAsync();
        }
        
        public async Task<MajorGroup> GetMajorGroupByIdAsync(Guid id)
        {
            return await _majorGroupRepo.GetMajorGroupByIdAsync(id);
        }
        
        public async Task<MajorGroup> CreateMajorGroupAsync(MajorGroupCreateDto majorGroupCreateDto)
        {
            var majorGroup = new MajorGroup
            {
                Name = majorGroupCreateDto.Name,
                DepartmentId = majorGroupCreateDto.DepartmentId,
                IsActive = true
            };
            
            return await _majorGroupRepo.CreateMajorGroupAsync(majorGroup);
        }
        
        public async Task<bool> DeactivateMajorGroupAsync(DeactivateDto deactivateDto)
        {
            return await _majorGroupRepo.DeactivateMajorGroupAsync(deactivateDto.Id);
        }
    }
}