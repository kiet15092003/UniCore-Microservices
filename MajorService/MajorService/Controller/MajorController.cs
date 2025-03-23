using MajorService.Business.Dtos.Major;
using MajorService.Business.Services.MajorServices;
using MajorService.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MajorService.Controller
{
    [Route("api/m/[controller]")]
    [ApiController]
    //[Authorize]
    public class MajorController : ControllerBase
    {
        private readonly IMajorSvc _majorSvc;

        public MajorController(IMajorSvc majorSvc)
        {
            _majorSvc = majorSvc;
        }

        [HttpGet]
        public async Task<ApiResponse<List<MajorReadDto>>> GetAllMajorAsync()
        {
            var majors = await _majorSvc.GetAllMajorAsync();
            return ApiResponse<List<MajorReadDto>>.SuccessResponse(majors);
        }
    }
}
