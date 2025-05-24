using CourseService.Business.Dtos.Shift;

namespace CourseService.Business.Services
{
    public interface IShiftService
    {
        Task<IEnumerable<ShiftDto>> GetAllShiftsAsync();
        Task<ShiftDto> GetShiftByIdAsync(Guid id);
    }
}
