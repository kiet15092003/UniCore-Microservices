using CourseService.Business.Dtos.Shift;
using CourseService.Business.Services;
using Microsoft.AspNetCore.Mvc;
using CourseService.Middleware;

namespace CourseService.Controllers
{
    [Route("api/c/[controller]")]
    [ApiController]
    public class ShiftsController : ControllerBase
    {
        private readonly IShiftService _shiftService;

        public ShiftsController(IShiftService shiftService)
        {
            _shiftService = shiftService;
        }

        [HttpGet]
        public async Task<ApiResponse<IEnumerable<ShiftDto>>> GetAllShifts()
        {
            var shifts = await _shiftService.GetAllShiftsAsync();
            return ApiResponse<IEnumerable<ShiftDto>>.SuccessResponse(shifts);
        }

        [HttpGet("{id}")]
        public async Task<ApiResponse<ShiftDto>> GetShiftById(Guid id)
        {
            var shift = await _shiftService.GetShiftByIdAsync(id);
            if (shift == null)
            {
                return ApiResponse<ShiftDto>.ErrorResponse(new List<string> { "Shift not found" });
            }
            return ApiResponse<ShiftDto>.SuccessResponse(shift);
        }
    }
}
