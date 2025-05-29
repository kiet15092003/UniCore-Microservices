using AutoMapper;
using CourseService.Business.Dtos.Shift;
using CourseService.DataAccess.Repositories;
using CourseService.Entities;

namespace CourseService.Business.Services
{
    public class ShiftService : IShiftService
    {
        private readonly IShiftRepository _shiftRepository;
        private readonly IMapper _mapper;

        public ShiftService(IShiftRepository shiftRepository, IMapper mapper)
        {
            _shiftRepository = shiftRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ShiftDto>> GetAllShiftsAsync()
        {
            var shifts = await _shiftRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ShiftDto>>(shifts);
        }

        public async Task<ShiftDto> GetShiftByIdAsync(Guid id)
        {
            var shift = await _shiftRepository.GetByIdAsync(id);
            return _mapper.Map<ShiftDto>(shift);
        }
    }
}
