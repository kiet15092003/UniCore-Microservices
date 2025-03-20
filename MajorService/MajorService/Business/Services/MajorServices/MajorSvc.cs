using AutoMapper;
using MajorService.Business.Dtos.Major;
using MajorService.DataAccess.Repositories.MajorRepo;

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
    }
}
