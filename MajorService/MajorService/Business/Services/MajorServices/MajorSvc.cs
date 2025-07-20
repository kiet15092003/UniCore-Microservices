using AutoMapper;
using MajorService.Business.Dtos;
using MajorService.Business.Dtos.Major;
using MajorService.DataAccess.Repositories.MajorRepo;
using MajorService.Entities;
using MajorService.Utils.Filter;
using MajorService.Utils.Pagination;

namespace MajorService.Business.Services.MajorServices
{
    public class MajorSvc : IMajorSvc
    {
        private readonly IMajorRepo _majorRepo;
        private readonly IMapper _mapper;
        public MajorSvc(IMajorRepo majorRepo, IMapper mapper)
        {
            _majorRepo = majorRepo;
            _mapper = mapper;
        }
        public async Task<List<MajorReadDto>> GetAllMajorAsync()
        {
            var majors = await _majorRepo.GetAllMajorAsync();
            return _mapper.Map<List<MajorReadDto>>(majors); 
        }
          public async Task<MajorReadDto> CreateMajorAsync(MajorCreateDto majorCreateDto)
        {
            var major = new Major
            {
                Name = majorCreateDto.Name,
                Code = majorCreateDto.Code,
                CostPerCredit = majorCreateDto.CostPerCredit,
                MajorGroupId = majorCreateDto.MajorGroupId,
                IsActive = true
            };
            
            var createdMajor = await _majorRepo.CreateMajorAsync(major);
            return _mapper.Map<MajorReadDto>(createdMajor);
        }
        
        public async Task<MajorReadDto> CreateNewMajorAsync(CreateNewMajorDto dto)
        {
            // Check if name already exists
            bool nameExists = await _majorRepo.IsMajorNameExistsAsync(dto.Name);
            if (nameExists)
            {
                throw new InvalidOperationException($"Major with name '{dto.Name}' already exists.");
            }
            
            // Generate a unique 6-digit code
            string uniqueCode = await _majorRepo.GenerateUniqueCodeAsync();
            
            // Create the major
            var major = new Major
            {
                Name = dto.Name,
                Code = uniqueCode,
                CostPerCredit = dto.CostPerCredit,
                MajorGroupId = dto.MajorGroupId,
                IsActive = true
            };
            
            var createdMajor = await _majorRepo.CreateMajorAsync(major);
            return _mapper.Map<MajorReadDto>(createdMajor);
        }

        public async Task<MajorReadDto> UpdateMajorAsync(Guid id, UpdateMajorDto dto)
        {
            // Check if major exists
            var existingMajor = await _majorRepo.GetMajorByIdAsync(id);
            if (existingMajor == null)
            {
                throw new KeyNotFoundException($"Major with ID '{id}' not found.");
            }
            
            // Check if name already exists for other majors
            bool nameExists = await _majorRepo.IsMajorNameExistsForOtherAsync(id, dto.Name);
            if (nameExists)
            {
                throw new InvalidOperationException($"Major with name '{dto.Name}' already exists.");
            }
            
            // Update the major
            existingMajor.Name = dto.Name;
            existingMajor.CostPerCredit = dto.CostPerCredit;
            if (dto.MajorGroupId.HasValue)
            {
                existingMajor.MajorGroupId = dto.MajorGroupId.Value;
            }
            
            var updatedMajor = await _majorRepo.UpdateMajorAsync(existingMajor);
            return _mapper.Map<MajorReadDto>(updatedMajor);
        }
          public async Task<bool> DeactivateMajorAsync(DeactivateDto deactivateDto)
        {
            return await _majorRepo.DeactivateMajorAsync(deactivateDto.Id);
        }
        
        public async Task<bool> ActivateMajorAsync(ActivateDto activateDto)
        {
            return await _majorRepo.ActivateMajorAsync(activateDto.Id);
        }
          public async Task<MajorListResponse> GetMajorsByPaginationAsync(Pagination pagination, MajorListFilterParams majorListFilterParams, Order? order)
        {
            var result = await _majorRepo.GetMajorsByPaginationAsync(pagination, majorListFilterParams, order);
            
            var response = new MajorListResponse
            {
                Data = _mapper.Map<List<MajorReadDto>>(result.Data),
                Total = result.Total,
                PageSize = result.PageSize,
                PageIndex = result.PageIndex
            };
            
            return response;
        }

        public async Task<bool> DeleteMajorAsync(Guid id)
        {
            // Check if major exists
            try
            {
                var major = await _majorRepo.GetMajorByIdAsync(id);
                // Major can always be deleted according to requirements
                var result = await _majorRepo.DeleteMajorAsync(id);
                return result;
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException("Major not found");
            }
        }
    }
}
