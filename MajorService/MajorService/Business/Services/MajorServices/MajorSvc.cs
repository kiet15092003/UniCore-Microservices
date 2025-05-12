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
        
        public async Task<bool> DeactivateMajorAsync(DeactivateDto deactivateDto)
        {
            return await _majorRepo.DeactivateMajorAsync(deactivateDto.Id);
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
    }
}
