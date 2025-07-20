using AutoMapper;
using MajorService.Business.Dtos;
using MajorService.Business.Dtos.MajorGroup;
using MajorService.DataAccess.Repositories.MajorGroupRepo;
using MajorService.Entities;
using MajorService.Utils.Filter;
using MajorService.Utils.Pagination;

namespace MajorService.Business.Services.MajorGroupServices
{
    public class MajorGroupSvc : IMajorGroupSvc
    {
        private readonly IMajorGroupRepo _majorGroupRepo;
        private readonly IMapper _mapper;
        
        public MajorGroupSvc(IMajorGroupRepo majorGroupRepo, IMapper mapper)
        {
            _majorGroupRepo = majorGroupRepo;
            _mapper = mapper;
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
        
        public async Task<MajorGroup> CreateNewMajorGroupAsync(CreateNewMajorGroupDto dto)
        {
            // Check if name already exists
            bool nameExists = await _majorGroupRepo.IsMajorGroupNameExistsAsync(dto.Name);
            if (nameExists)
            {
                throw new InvalidOperationException($"Major group with name '{dto.Name}' already exists.");
            }
            
            // Generate a unique 6-digit code
            string uniqueCode = await _majorGroupRepo.GenerateUniqueCodeAsync();
            
            // Create the major group
            var majorGroup = new MajorGroup
            {
                Name = dto.Name,
                Code = uniqueCode,
                DepartmentId = dto.DepartmentId,
                IsActive = true
            };
            
            return await _majorGroupRepo.CreateMajorGroupAsync(majorGroup);
        }

        public async Task<MajorGroup> UpdateMajorGroupAsync(Guid id, UpdateMajorGroupDto dto)
        {
            // Check if major group exists
            var existingMajorGroup = await _majorGroupRepo.GetMajorGroupByIdAsync(id);
            if (existingMajorGroup == null)
            {
                throw new KeyNotFoundException($"Major group with ID '{id}' not found.");
            }
            
            // Check if name already exists for other major groups
            bool nameExists = await _majorGroupRepo.IsMajorGroupNameExistsForOtherAsync(id, dto.Name);
            if (nameExists)
            {
                throw new InvalidOperationException($"Major group with name '{dto.Name}' already exists.");
            }
            
            // Update the major group
            existingMajorGroup.Name = dto.Name;
            if (dto.DepartmentId.HasValue)
            {
                existingMajorGroup.DepartmentId = dto.DepartmentId.Value;
            }
            
            return await _majorGroupRepo.UpdateMajorGroupAsync(existingMajorGroup);
        }
          public async Task<bool> DeactivateMajorGroupAsync(DeactivateDto deactivateDto)
        {
            return await _majorGroupRepo.DeactivateMajorGroupAsync(deactivateDto.Id);
        }
        
        public async Task<bool> ActivateMajorGroupAsync(ActivateDto activateDto)
        {
            return await _majorGroupRepo.ActivateMajorGroupAsync(activateDto.Id);
        }
        
        public async Task<MajorGroupListResponse> GetMajorGroupsByPaginationAsync(
            Pagination pagination, 
            MajorGroupListFilterParams majorGroupListFilterParams, 
            Order? order)
        {
            var result = await _majorGroupRepo.GetMajorGroupsByPaginationAsync(
                pagination, 
                majorGroupListFilterParams, 
                order);
            
            var dtos = _mapper.Map<List<MajorGroupReadDto>>(result.Data);
            
            var response = new MajorGroupListResponse
            {
                Data = dtos,
                Total = result.Total,
                PageSize = result.PageSize,
                PageIndex = result.PageIndex
            };
            
            return response;
        }

        public async Task<bool> DeleteMajorGroupAsync(Guid id)
        {
            // Check if major group exists
            try
            {
                var majorGroup = await _majorGroupRepo.GetMajorGroupByIdAsync(id);
                
                // Check if major group has any majors
                var majors = await _majorGroupRepo.GetMajorsByMajorGroupIdAsync(id);
                if (majors.Any())
                {
                    throw new InvalidOperationException($"Cannot delete major group '{majorGroup.Name}' because it has {majors.Count} associated major(s). Please remove all majors before deleting the major group.");
                }

                // If no majors, proceed with deletion
                var result = await _majorGroupRepo.DeleteMajorGroupAsync(id);
                return result;
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException("Major group not found");
            }
        }
    }
}