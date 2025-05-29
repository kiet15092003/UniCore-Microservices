using AutoMapper;
using CourseService.Business.Dtos.Course;
using CourseService.Business.Dtos.Material;
using CourseService.DataAccess.Repositories;
using CourseService.Entities;
using CourseService.Middleware;
using CourseService.Utils;
using CourseService.Utils.Filter;
using CourseService.Utils.Pagination;

namespace CourseService.Business.Services
{
    public class CourseMaterialService : ICourseMaterialService
    {
        private readonly ICourseMaterialRepository _repository;
        private readonly ICourseRepository _courseRepository;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IMapper _mapper;

        public CourseMaterialService(
            ICourseMaterialRepository repository,
            ICourseRepository courseRepository,
            ICloudinaryService cloudinaryService,
            IMapper mapper)
        {
            _repository = repository;
            _courseRepository = courseRepository;
            _cloudinaryService = cloudinaryService;
            _mapper = mapper;
        }

        public async Task<ApiResponse<PaginationResult<MaterialReadDto>>> GetMaterialsPaginationAsync(
            Guid courseId,
            Pagination pagination,
            MaterialListFilterParams filterParams,
            Order? order)
        {
            // Kiểm tra course có tồn tại không
            var course = await _courseRepository.GetCourseByIdAsync(courseId);
            if (course == null)
                return new ApiResponse<PaginationResult<MaterialReadDto>>(
                    false, 
                    null, 
                    new List<string> { "Course not found" });

            // Lấy danh sách material với phân trang
            var paginationResult = await _repository.GetMaterialsPaginationAsync(
                courseId, 
                pagination, 
                filterParams, 
                order);

            // Map kết quả sang DTO
            var materialDtos = _mapper.Map<List<MaterialReadDto>>(paginationResult.Data);
            
            var result = new PaginationResult<MaterialReadDto>
            {
                Data = materialDtos,
                Total = paginationResult.Total,
                PageIndex = paginationResult.PageIndex,
                PageSize = paginationResult.PageSize
            };

            return new ApiResponse<PaginationResult<MaterialReadDto>>(true, result);
        }

        public async Task<ApiResponse<IEnumerable<CourseMaterialReadDto>>> GetMaterialsByCourseIdAsync(Guid courseId)
        {
            var course = await _courseRepository.GetCourseByIdAsync(courseId);
            if (course == null)
                return new ApiResponse<IEnumerable<CourseMaterialReadDto>>(false, null, new List<string> { "Course not found" });

            var courseMaterials = await _repository.GetByCourseIdAsync(courseId);
            var courseMaterialDtos = _mapper.Map<IEnumerable<CourseMaterialReadDto>>(courseMaterials);

            return new ApiResponse<IEnumerable<CourseMaterialReadDto>>(true, courseMaterialDtos);
        }

        public async Task<ApiResponse<CourseMaterialReadDto>> GetMaterialByIdAsync(Guid courseId, Guid materialId)
        {
            var courseMaterial = await _repository.GetByIdAsync(courseId, materialId);
            if (courseMaterial == null)
                return new ApiResponse<CourseMaterialReadDto>(false, null, new List<string> { "Course material not found" });

            var courseMaterialDto = _mapper.Map<CourseMaterialReadDto>(courseMaterial);
            return new ApiResponse<CourseMaterialReadDto>(true, courseMaterialDto);
        }

        public async Task<ApiResponse<CourseMaterialReadDto>> AddMaterialAsync(CourseMaterialCreateDto createDto, Guid courseId)
        {
            var course = await _courseRepository.GetCourseByIdAsync(courseId);
            if (course == null)
                return new ApiResponse<CourseMaterialReadDto>(false, null, new List<string> { "Course not found" });

            // Upload file to Cloudinary
            var fileUrl = await _cloudinaryService.UploadFileAsync(createDto.File);
            if (string.IsNullOrEmpty(fileUrl))
                return new ApiResponse<CourseMaterialReadDto>(false, null, new List<string> { "Failed to upload file" });

            // Create new material
            var material = new Material
            {
                Name = createDto.Name,
                FileUrl = fileUrl,
                MaterialTypeId = createDto.MaterialTypeId
            };

            var addedMaterial = await _repository.AddMaterialAsync(material);

            // Create course material relationship
            var courseMaterial = new CourseMaterial
            {
                CourseId = courseId,
                MaterialId = addedMaterial.Id
            };

            await _repository.AddAsync(courseMaterial);

            // Get the added material with course relationship
            var result = await _repository.GetByIdAsync(courseId, addedMaterial.Id);
            var courseMaterialDto = _mapper.Map<CourseMaterialReadDto>(result);

            return new ApiResponse<CourseMaterialReadDto>(true, courseMaterialDto);
        }

        public async Task<ApiResponse<bool>> UpdateMaterialAsync(Guid courseId, Guid materialId, CourseMaterialUpdateDto updateDto)
        {
            var courseMaterial = await _repository.GetByIdAsync(courseId, materialId);
            if (courseMaterial == null)
                return new ApiResponse<bool>(false, false, new List<string> { "Course material not found" });

            var material = await _repository.GetMaterialByIdAsync(materialId);
            if (material == null)
                return new ApiResponse<bool>(false, false, new List<string> { "Material not found" });

            // Update material name
            material.Name = updateDto.Name;
            
            // Update material type if provided
            if (updateDto.MaterialTypeId.HasValue)
            {
                material.MaterialTypeId = updateDto.MaterialTypeId;
            }

            // If new file is provided, upload it and update the URL
            if (updateDto.File != null)
            {
                var fileUrl = await _cloudinaryService.UploadFileAsync(updateDto.File);
                if (string.IsNullOrEmpty(fileUrl))
                    return new ApiResponse<bool>(false, false, new List<string> { "Failed to upload file" });

                material.FileUrl = fileUrl;
            }

            // Update the material
            var result = await _repository.UpdateMaterialAsync(material);
            
            return new ApiResponse<bool>(true, result);
        }

        public async Task<ApiResponse<bool>> DeleteMaterialAsync(Guid courseId, Guid materialId)
        {
            var courseMaterial = await _repository.GetByIdAsync(courseId, materialId);
            if (courseMaterial == null)
                return new ApiResponse<bool>(false, false, new List<string> { "Course material not found" });

            // Delete the course-material relationship
            var deleteResult = await _repository.DeleteAsync(courseId, materialId);
            if (!deleteResult)
                return new ApiResponse<bool>(false, false, new List<string> { "Failed to delete course material" });

            return new ApiResponse<bool>(true, true);
        }
    }
} 