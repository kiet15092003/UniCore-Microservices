using CourseService.Business.Dtos.Course;
using CourseService.Business.Dtos.Material;
using CourseService.Business.Services;
using CourseService.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseService.Controllers
{
    [Route("api/courses/{courseId}/materials")]
    [ApiController]
    // [Authorize] - Comment lại để tắt xác thực
    public class MaterialsController : ControllerBase
    {
        private readonly ICourseMaterialService _materialService;

        public MaterialsController(ICourseMaterialService courseMaterialService)
        {
            _materialService = courseMaterialService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<CourseMaterialReadDto>>>> GetMaterials(Guid courseId)
        {
            var response = await _materialService.GetMaterialsByCourseIdAsync(courseId);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("{materialId}")]
        public async Task<ActionResult<ApiResponse<CourseMaterialReadDto>>> GetMaterial(Guid courseId, Guid materialId)
        {
            var response = await _materialService.GetMaterialByIdAsync(courseId, materialId);
            if (!response.Success)
                return NotFound(response);

            return Ok(response);
        }

        [HttpPost]
        // [Authorize(Roles = "Admin")] - Comment lại để tắt xác thực role
        public async Task<ActionResult<ApiResponse<CourseMaterialReadDto>>> CreateMaterial([FromForm] CourseMaterialCreateDto createDto, Guid courseId)
        {
            // Tự động gán courseId từ URL vào DTO
            createDto.CourseId = courseId;

            var response = await _materialService.AddMaterialAsync(createDto);
            if (!response.Success)
                return BadRequest(response);

            return CreatedAtAction(nameof(GetMaterial), new { courseId, materialId = response.Data.MaterialId }, response);
        }

        [HttpPut("{materialId}")]
        // [Authorize(Roles = "Admin,Teacher")] - Comment lại để tắt xác thực role
        public async Task<ActionResult<ApiResponse<CourseMaterialReadDto>>> UpdateMaterial([FromForm] CourseMaterialUpdateDto updateDto, Guid courseId, Guid materialId)
        {
            // Ensure the materialId in the URL matches the one in the DTO
            updateDto.MaterialId = materialId;

            var response = await _materialService.UpdateMaterialAsync(courseId, updateDto);
            if (!response.Success)
                return NotFound(response);

            return Ok(response);
        }

        [HttpDelete("{materialId}")]
        // [Authorize(Roles = "Admin,Teacher")] - Comment lại để tắt xác thực role
        public async Task<ActionResult<ApiResponse<bool>>> DeleteMaterial(Guid courseId, Guid materialId)
        {
            var response = await _materialService.DeleteMaterialAsync(courseId, materialId);
            if (!response.Success)
                return NotFound(response);

            return Ok(response);
        }
    }
} 