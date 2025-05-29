using AutoMapper;
using CourseService.Business.Dtos.Material;
using CourseService.DataAccess;
using CourseService.Entities;
using CourseService.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Controllers
{
    [Route("api/c/Courses/material-types")]
    [ApiController]
    // [Authorize] - Comment lại để tắt xác thực
    public class MaterialTypesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;

        public MaterialTypesController(IMapper mapper, AppDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<MaterialTypeReadDto>>>> GetAllMaterialTypes()
        {
            var materialTypes = await _context.MaterialTypes.ToListAsync();
            var materialTypeDtos = _mapper.Map<List<MaterialTypeReadDto>>(materialTypes);
            return Ok(new ApiResponse<List<MaterialTypeReadDto>>(true, materialTypeDtos));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<MaterialTypeReadDto>>> GetMaterialTypeById(Guid id)
        {
            var materialType = await _context.MaterialTypes.FindAsync(id);
            if (materialType == null)
                return NotFound(new ApiResponse<MaterialTypeReadDto>(false, null, new List<string> { "Material type not found" }));

            var materialTypeDto = _mapper.Map<MaterialTypeReadDto>(materialType);
            return Ok(new ApiResponse<MaterialTypeReadDto>(true, materialTypeDto));
        }

        [HttpPost]
        // [Authorize(Roles = "Admin")] - Comment lại để tắt xác thực role
        public async Task<ActionResult<ApiResponse<MaterialTypeReadDto>>> CreateMaterialType([FromBody] MaterialTypeCreateDto createDto)
        {
            var materialType = _mapper.Map<MaterialType>(createDto);
            
            await _context.MaterialTypes.AddAsync(materialType);
            await _context.SaveChangesAsync();
            
            var materialTypeDto = _mapper.Map<MaterialTypeReadDto>(materialType);
            return CreatedAtAction(nameof(GetMaterialTypeById), new { id = materialType.Id }, new ApiResponse<MaterialTypeReadDto>(true, materialTypeDto));
        }

        [HttpPut("{id}")]
        // [Authorize(Roles = "Admin")] - Comment lại để tắt xác thực role
        public async Task<ActionResult<ApiResponse<MaterialTypeReadDto>>> UpdateMaterialType(Guid id, [FromBody] MaterialTypeUpdateDto updateDto)
        {
            if (id != updateDto.Id)
                return BadRequest(new ApiResponse<MaterialTypeReadDto>(false, null, new List<string> { "Id in URL does not match Id in request body" }));

            var materialType = await _context.MaterialTypes.FindAsync(id);
            if (materialType == null)
                return NotFound(new ApiResponse<MaterialTypeReadDto>(false, null, new List<string> { "Material type not found" }));

            _mapper.Map(updateDto, materialType);
            
            _context.MaterialTypes.Update(materialType);
            await _context.SaveChangesAsync();
            
            var materialTypeDto = _mapper.Map<MaterialTypeReadDto>(materialType);
            return Ok(new ApiResponse<MaterialTypeReadDto>(true, materialTypeDto));
        }

        [HttpDelete("{id}")]
        // [Authorize(Roles = "Admin")] - Comment lại để tắt xác thực role
        public async Task<ActionResult<ApiResponse<bool>>> DeleteMaterialType(Guid id)
        {
            var materialType = await _context.MaterialTypes.FindAsync(id);
            if (materialType == null)
                return NotFound(new ApiResponse<bool>(false, false, new List<string> { "Material type not found" }));

            // Check if any materials are using this type
            var isUsed = await _context.Materials.AnyAsync(m => m.MaterialTypeId == id);
            if (isUsed)
                return BadRequest(new ApiResponse<bool>(false, false, new List<string> { "Cannot delete material type that is in use" }));

            _context.MaterialTypes.Remove(materialType);
            await _context.SaveChangesAsync();
            
            return Ok(new ApiResponse<bool>(true, true));
        }
    }
} 